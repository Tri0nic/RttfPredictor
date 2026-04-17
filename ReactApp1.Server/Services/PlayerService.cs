using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly RttfLinks _rttfLinks;
        private readonly ILogger<PlayerService> _logger;

        public PlayerService(IPlayerRepository playerRepository, IOptions<RttfLinks> rttfLinks, ILogger<PlayerService> logger)
        {
            _playerRepository = playerRepository;
            _rttfLinks = rttfLinks.Value;
            _logger = logger;
        }

        public async Task<(MethodResult, string, List<PlayerStats>)> GetTournamentPlayers()
        {
            var (result, message, response) = await _playerRepository.GetTournamentPlayersStats();

            return (result, message, response);
        }

        public async Task<(MethodResult, string, Dictionary<string, Dictionary<long, List<PlayerStats>>>)> PostTournamentsPlayersStatsNearbyDays(int startDay, int endDay)
        {
            var now = DateTime.Now;
            var result = new Dictionary<string, Dictionary<long, List<PlayerStats>>>();

            for (int i = startDay; i <= endDay; i++)
            {
                var selectedDay = now.Date.AddDays(i).ToString("dd.MM.yyyy");

                _logger.LogInformation($"Началось добавление игроков за турниры {selectedDay} числа");

                var (dayResult, dayMessage, dayPlayers) = await PostTournamentsPlayersStats(i, i);

                result[selectedDay] = dayPlayers ?? new Dictionary<long, List<PlayerStats>>();

                _logger.LogInformation($"Закончилось добавление игроков за турниры {selectedDay} числа");
            }

            return (MethodResult.Success, "", result);
        }

        private async Task<(MethodResult, string, Dictionary<long, List<PlayerStats>>)> PostTournamentsPlayersStats(int startDay, int endDay)
        {
            var now = DateTime.Now;

            var baseUrl = _rttfLinks.SingleMoscowTournamentsLinks
                .Replace("date_from=", $"date_from={now.Date.AddDays(startDay):dd.MM.yyyy}")
                .Replace("date_to=", $"date_to={now.Date.AddDays(endDay):dd.MM.yyyy}");

            var web = new HtmlWeb();
            var tournamentsDoc = web.Load(baseUrl);

            var rows = tournamentsDoc.DocumentNode.SelectNodes(
                    "//section[contains(@class, 'tours-list')]//tr[contains(@onclick, '/tournaments/')]"
                );

            if (rows == null || !rows.Any())
                return (MethodResult.NotFound, "Турниры не найдены", null);

            var tournamentIds = rows
                .Select(r => System.Text.RegularExpressions.Regex.Match(
                    r.GetAttributeValue("onclick", ""), @"/tournaments/(\d+)"))
                .Where(m => m.Success)
                .Select(m => long.Parse(m.Groups[1].Value))
                .Distinct()
                .ToList();

            var result = new Dictionary<long, List<PlayerStats>>();

            var i = 0;
            foreach (var tournamentId in tournamentIds)
            {
                _logger.LogInformation($"Турнир {tournamentId} -- {++i}/{tournamentIds.Count}");

                var (resultStats, messageStats, playerStats) = await PostTournamentPlayersStats(tournamentId.ToString());
                if (resultStats == MethodResult.Success && playerStats != null)
                    result[tournamentId] = playerStats;
            }

            return (MethodResult.Success, "", result);
        }

        public async Task<(MethodResult, string, List<PlayerStats>)> PostTournamentPlayersStats(string tournamentLink)
        {
            var (resultTournamentStatus, messageTournamentStatus, tournamentStatus) = await GetTournamentStatus(tournamentLink);
            if (resultTournamentStatus != MethodResult.Success)
            {
                return (resultTournamentStatus, messageTournamentStatus, null);
            }
            
            var (resultParsed, messageParsed, responseParsed) = await ParseTournamentPlayersStats(tournamentLink, tournamentStatus);
            if (resultParsed != MethodResult.Success)
            {
                return (resultParsed, messageParsed, null);
            }

            var (resultSave, messageSave) = await SaveTournamentPlayersStats(responseParsed, tournamentStatus);
            if (resultSave != MethodResult.Success)
            {
                return (resultSave, messageSave, null);
            }

            return (MethodResult.Success, messageSave, responseParsed);
        }

        private async Task<(MethodResult, string, TournamentStatus?)> GetTournamentStatus(string tournamentLink)
        {
            var web = new HtmlWeb();
            var baseUri = new Uri(_rttfLinks.CalendarLink);
            var baseUrl = $"{baseUri.Scheme}://{baseUri.Host}/";

            var tournamentDoc = web.Load(baseUrl + "tournaments/" + tournamentLink);

            if (tournamentDoc.DocumentNode.SelectSingleNode("//section[contains(@class, 'tour-online') and not(@hidden)]") != null)
            {
                return (MethodResult.Success, "", TournamentStatus.Live);
            }
            else if (tournamentDoc.DocumentNode.SelectSingleNode("//section[contains(@class, 'tour-results') and not(@hidden)]") != null)
            {
                return (MethodResult.Success, "", TournamentStatus.Ended);
            }
            else if (tournamentDoc.DocumentNode.SelectSingleNode("//section[contains(@class, 'tour-desc') and not(@hidden)]") != null)
            {
                return (MethodResult.Success, "", TournamentStatus.NotStarted);
            }
            else
            {
                return (MethodResult.NotFound, "Статус турнира не определен", null);
            }
        }

        private async Task<(MethodResult, string, List<PlayerStats>)> ParseTournamentPlayersStats(string tournamentLink, TournamentStatus? tournamentStatus)
        {
            if (tournamentStatus == TournamentStatus.Ended)
            {
                if (!long.TryParse(tournamentLink, out var tournamentId) ||
                    !await _playerRepository.TournamentExists(tournamentId))
                {
                    return (MethodResult.NotFound, $"Турнир {tournamentId} не найден в БД — сохранение результатов невозможно", null);
                }

                var (resultTournamentEndedStats, messageTournamentEndedStats, tournamentEndedStats) = await ParseEndedTournamentPlayersStats(tournamentLink);

                return (resultTournamentEndedStats, messageTournamentEndedStats, tournamentEndedStats);
            }
            else if (tournamentStatus == TournamentStatus.Live)
            {
                _logger.LogInformation("Турнир в процессе");
                return (MethodResult.Success, "Турнир в процессе", null);
            }
            else if (tournamentStatus == TournamentStatus.NotStarted)
            {
                var (resultTournamentNotStartedStats, messageTournamentNotStartedStats, tournamentNotStartedStats) = await ParseNotStartedTournamentPlayersStats(tournamentLink);

                return (resultTournamentNotStartedStats, messageTournamentNotStartedStats, tournamentNotStartedStats);
            }
            else
            {
                return (MethodResult.NotFound, "Не удалось спарсить игроков турнира", null);
            }
        }

        private async Task<(MethodResult, string, List<PlayerStats>)> ParseEndedTournamentPlayersStats(string tournamentLink)
        {
            var web = new HtmlWeb();
            var baseUri = new Uri(_rttfLinks.CalendarLink);
            var baseUrl = $"{baseUri.Scheme}://{baseUri.Host}/";

            var tournamentDoc = web.Load(baseUrl + "tournaments/" + tournamentLink);

            var playersStats = new List<PlayerStats>();

            var playerNodes = tournamentDoc.DocumentNode.SelectNodes(
                "//section[@class='tour-results']//table[contains(@class, 'tour-players')]//tbody//tr//a[contains(@href, 'players/')]"
            );

            if (playerNodes == null) return (MethodResult.Success, "", playersStats);

            var playerEntries = playerNodes
                .Select(n => n.GetAttributeValue("href", "").Split('?')[0].Replace("players/", ""))
                .Where(p => !string.IsNullOrEmpty(p) && long.TryParse(p, out _))
                .Distinct()
                .ToList();

            if (!long.TryParse(tournamentLink, out var tournamentId))
                return (MethodResult.NotFound, "Некорректный идентификатор турнира", null);

            var tournamentDate = ParseTournamentDate(tournamentDoc);

            var position = 1;
            foreach (var playerLink in playerEntries)
            {
                playersStats.Add(new PlayerStats
                {
                    PlayerId = long.Parse(playerLink),
                    TournamentId = tournamentId,
                    Position = position,
                    TournamentDate = tournamentDate
                });

                position++;
            }

            return (MethodResult.Success, "", playersStats);
        }

        private async Task<(MethodResult, string, List<PlayerStats>)> ParseNotStartedTournamentPlayersStats(string tournamentLink)
        {
            var web = new HtmlWeb();
            var baseUri = new Uri(_rttfLinks.CalendarLink);
            var baseUrl = $"{baseUri.Scheme}://{baseUri.Host}/";

            var tournamentDoc = web.Load(baseUrl + "tournaments/" + tournamentLink);

            var playersStats = new List<PlayerStats>();

            var playerNodes = tournamentDoc.DocumentNode.SelectNodes(
                "//section[@id='tour-reg-list']//table[@class='tablesort']//tbody//tr//a[contains(@href, 'players/')]"
            );

            if (playerNodes == null)
            {
                return (MethodResult.NotFound, "В турнире нет участников", playersStats);
            }

            if (!long.TryParse(tournamentLink, out var tournamentId))
                return (MethodResult.NotFound, "Некорректный идентификатор турнира", null);

            var tournamentDate = ParseTournamentDate(tournamentDoc);

            var playerEntries = playerNodes
                .Select(n => n.GetAttributeValue("href", "").Split('?')[0])
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToList();

            foreach (var playerLink in playerEntries)
            {
                var doc = web.Load(baseUrl + playerLink);

                var h3Node = doc.DocumentNode.SelectSingleNode(
                    "//section[@class='player-info']/h3"
                );
                var nick = h3Node?.ChildNodes
                    .FirstOrDefault(n => n.NodeType == HtmlAgilityPack.HtmlNodeType.Text
                                      && !string.IsNullOrWhiteSpace(n.InnerText))
                    ?.InnerText.Trim().Trim('"') ?? "";

                var rating = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (i > 0)
                    {
                        _logger.LogWarning($"Не удалось спарсить рейтинг пользователя {nick}. \nПовторная попытка...");
                        await Task.Delay(300);
                    }

                    var ratingMatch = System.Text.RegularExpressions.Regex.Match(
                        h3Node?.SelectSingleNode("dfn")?.InnerText ?? "", @"\d+"
                    );
                    int.TryParse(ratingMatch.Value, out rating);

                    if (rating != 0)
                    {
                        break;
                    }

                }

                var infoParagraphs = doc.DocumentNode.SelectNodes(
                    "//section[@class='player-info']/p"
                );

                var yearParagraph = infoParagraphs?.FirstOrDefault(p => p.InnerText.Contains("рождения"));
                var yearMatch = System.Text.RegularExpressions.Regex.Match(
                    yearParagraph?.InnerText ?? "", @"(\d{4})"
                );
                int.TryParse(yearMatch.Groups[1].Value, out var year);

                var cityRaw = infoParagraphs?.FirstOrDefault(p => p.InnerText.Contains("город"))
                    ?.SelectSingleNode(".//strong")?.InnerText.Trim();
                var city = System.Text.RegularExpressions.Regex.Replace(
                    cityRaw ?? "", @"\s*\([^)]*\)", ""
                ).Trim();

                var arm = infoParagraphs?.FirstOrDefault(p => p.InnerText.Contains("рука"))
                    ?.SelectSingleNode(".//strong")?.InnerText.Trim();

                var statsRows = doc.DocumentNode.SelectNodes(
                    "//section[contains(@class,'player-stats')]/table//tr"
                );

                int tournamentsPlayed = 0, wonGames = 0, lostGames = 0;

                if (statsRows != null)
                {
                    foreach (var row in statsRows)
                    {
                        var cells = row.SelectNodes("td");
                        if (cells == null || cells.Count < 2) continue;

                        for (int i = 0; i + 1 < cells.Count; i += 2)
                        {
                            var label = cells[i].InnerText.Trim();
                            var value = cells[i + 1].InnerText.Replace("'", "").Trim();

                            if (label.Contains("Сыграно турниров") && tournamentsPlayed == 0)
                                int.TryParse(value, out tournamentsPlayed);
                            else if (label.Contains("Игры") && wonGames == 0 && lostGames == 0)
                            {
                                var m = System.Text.RegularExpressions.Regex.Match(value, @"\((\d+) - (\d+)\)");
                                int.TryParse(m.Groups[1].Value, out wonGames);
                                int.TryParse(m.Groups[2].Value, out lostGames);
                            }
                        }
                    }
                }

                var playerIdStr = playerLink.Replace("players/", "");
                if (!long.TryParse(playerIdStr, out var playerId))
                    continue;

                playersStats.Add(new PlayerStats
                {
                    PlayerId = playerId,
                    TournamentId = tournamentId,
                    Name = nick,
                    City = string.IsNullOrEmpty(city) ? null : city,
                    Arm = arm,
                    Year = year == 0 ? null : year,
                    Rating = rating,
                    TournamentsPlayed = tournamentsPlayed,
                    WonGames = wonGames,
                    LostGames = lostGames,
                    TournamentDate = tournamentDate
                });
            }

            return (MethodResult.Success, "", playersStats);
        }

        private static DateTime? ParseTournamentDate(HtmlAgilityPack.HtmlDocument doc)
        {
            var timeNode = doc.DocumentNode.SelectSingleNode("//time");
            if (timeNode == null) return null;

            var text = timeNode.InnerText;
            var dateMatch = System.Text.RegularExpressions.Regex.Match(text, @"\d{2}\.\d{2}\.\d{4}");
            var timeMatch = System.Text.RegularExpressions.Regex.Match(text, @"\d{2}:\d{2}");

            if (dateMatch.Success && timeMatch.Success &&
                DateTime.TryParseExact(
                    $"{dateMatch.Value} {timeMatch.Value}",
                    "dd.MM.yyyy HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out var dt))
            {
                return new DateTimeOffset(dt, TimeSpan.FromHours(3)).UtcDateTime;
            }

            return null;
        }

        private async Task<(MethodResult, string)> SaveTournamentPlayersStats(List<PlayerStats> responseParsed, TournamentStatus? tournamentStatus)
        {
            if (tournamentStatus == TournamentStatus.Ended)
            {
                var (resultSave, messageSave) = await _playerRepository.SaveTournamentResults(responseParsed);

                return (MethodResult.Success, messageSave);
            }
            else if (tournamentStatus == TournamentStatus.Live)
            {
                return (MethodResult.Success, "Турнир идет - сохранение не нужно");
            }
            else if (tournamentStatus == TournamentStatus.NotStarted)
            {
                var tournamentId = responseParsed.First().TournamentId;
                var startsAt = responseParsed.First().TournamentDate;
                await _playerRepository.UpsertTournament(tournamentId, startsAt);

                var (resultSave, messageSave) = await _playerRepository.SaveNotStartedTournamentPlayersStats(responseParsed);

                return (MethodResult.Success, messageSave);
            }
            else
            {
                return (MethodResult.NotFound, "Не удалось сохранить данные игроков турнира");
            }
        }
    }
}
