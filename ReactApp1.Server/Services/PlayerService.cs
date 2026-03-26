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

        public async Task<(MethodResult, string, PlayerResponse)> GetPlayer(int id)
        {
            var (result, message, response) = await _playerRepository.GetPlayer(id);

            return (result, message, response);
        }

        public async Task<(MethodResult, string, List<PlayerResponse>)> GetPlayers(GetPlayersRequest request)
        {
            var (result, message, response) = await _playerRepository.GetPlayers(request);

            return (result, message, response);
        }

        public async Task<(MethodResult, string, int)> PostPlayers(PostPlayersRequest request)
        {
            var (resultParseTournaments, messageParseTournaments, tournamentLinks) = await ParseTournaments(request);

            var (resultGetPlayersFromTournaments, messageGetPlayersFromTournaments, playersAfterTournaments) = await GetPlayersFromTournaments(tournamentLinks);


            var (result, message) = await _playerRepository.SavePlayersAfterTournaments(playersAfterTournaments);

            return (MethodResult.Success, "", playersAfterTournaments.Count);
        }


        private async Task<(MethodResult, string, List<string>)> ParseTournaments(PostPlayersRequest request)
        {
            var web = new HtmlWeb();


            if (request.Month.HasValue)
            {
                var month = request.Month >= 10 ? request.Month.ToString() : "0" + request.Month.ToString();
                _rttfLinks.CalendarLink = _rttfLinks.CalendarLink.Replace("month=", $"month={month}");
            }

            if (request.Year.HasValue)
            {
                _rttfLinks.CalendarLink = _rttfLinks.CalendarLink.Replace("year=", $"year={request.Year}");
            }

            if (request.Type != null)
            {
                _rttfLinks.CalendarLink = _rttfLinks.CalendarLink.Replace("rats=", $"rats={request.Type}");
            }

            if (request.City != null)
            {
                _rttfLinks.CalendarLink = _rttfLinks.CalendarLink.Replace("cities%5B%5D=", $"cities%5B%5D=r{request.City}");
            }

            if (request.RatingMin.HasValue)
            {
                _rttfLinks.CalendarLink = _rttfLinks.CalendarLink.Replace("rat_from=", $"rat_from={request.RatingMin}");
            }

            if (request.RatingMax.HasValue)
            {
                _rttfLinks.CalendarLink = _rttfLinks.CalendarLink.Replace("rat_to=", $"rat_to={request.RatingMax}");
            }

            var doc = web.Load(_rttfLinks.CalendarLink);
            var tournamentNodes = doc.DocumentNode.SelectNodes(
                "//table[@class='calendar']//a[contains(@href, 'tournaments/')]"
            );

            if (tournamentNodes == null || !tournamentNodes.Any())
            {
                return (MethodResult.NotFound, "Турниры не найдены", null);
            }

            var tournamentLinks = new List<string>();

            foreach (var node in tournamentNodes)
            {
                var href = node.GetAttributeValue("href", "");
                tournamentLinks.Add(href);
            }

            return (MethodResult.Success, "", tournamentLinks);
        }

        private async Task<(MethodResult, string, List<PlayerAfterTournament>)> GetPlayersFromTournaments(List<string> tournamentLinks)
        {
            var web = new HtmlWeb();
            var baseUri = new Uri(_rttfLinks.CalendarLink);
            var baseUrl = $"{baseUri.Scheme}://{baseUri.Host}/";
            var players = new List<PlayerAfterTournament>();

            foreach (var link in tournamentLinks)
            {
                var doc = web.Load(baseUrl + link);
                var rows = doc.DocumentNode.SelectNodes(
                    "//table[contains(@class, 'tour-players')]//tbody//tr"
                );

                if (rows == null) continue;

                foreach (var row in rows)
                {
                    var cells = row.SelectNodes("td");
                    if (cells == null) continue;

                    var playerLink = cells[1].SelectSingleNode(".//a")?.GetAttributeValue("href", "") ?? "";
                    var rawName = cells[1].SelectSingleNode(".//a")?.InnerText.Trim() ?? "";
                    var nickMatch = System.Text.RegularExpressions.Regex.Match(rawName, @"\(([^)]+)\)");
                    var name = nickMatch.Success ? nickMatch.Groups[1].Value : rawName;
                    var city = cells[2].InnerText.Trim();

                    int.TryParse(cells[3].SelectSingleNode(".//dfn")?.InnerText.Trim(), out var ratingBefore);
                    decimal.TryParse(cells[4].GetAttributeValue("data-sort", "0"),
                        System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var ratingDelta);
                    int.TryParse(cells[5].SelectSingleNode(".//dfn")?.InnerText.Trim(), out var ratingAfter);

                    var (games, gamesWon, gamesLost) = ParseStats(cells[6]);
                    var (sets, setsWon, setsLost) = ParseStats(cells[7]);

                    players.Add(new PlayerAfterTournament
                    {
                        Link = playerLink,
                        Name = name,
                        City = city,
                        RatingBefore = ratingBefore,
                        RatingDelta = ratingDelta,
                        RatingAfter = ratingAfter,
                        Games = games,
                        GamesWon = gamesWon,
                        GamesLost = gamesLost,
                        Sets = sets,
                        SetsWon = setsWon,
                        SetsLost = setsLost
                    });
                }
            }

            return (MethodResult.Success, "", players);
        }

        public async Task<(MethodResult, string, List<PlayerStats>)> PostTournamentPlayersStats(string tournamentLink)
        {
            var(resultParsed, messageParsed, responseParsed) = await ParseTournamentPlayersStats(tournamentLink);

            var(resultSave, messageSave) = await _playerRepository.SaveTournamentPlayersStats(responseParsed);

            return (MethodResult.Success, "", responseParsed);
        }

        private async Task<(MethodResult, string, List<PlayerStats>)> ParseTournamentPlayersStats(string tournamentLink)
        {
            var web = new HtmlWeb();
            var baseUri = new Uri(_rttfLinks.CalendarLink);
            var baseUrl = $"{baseUri.Scheme}://{baseUri.Host}/";
            var playersStats = new List<PlayerStats>();

            var tournamentDoc = web.Load(baseUrl + "tournaments/" + tournamentLink);
            var playerNodes = tournamentDoc.DocumentNode.SelectNodes(
                "//table[contains(@class, 'tour-players')]//tbody//tr//a[contains(@href, 'players/')]"
            );

            if (playerNodes == null) return (MethodResult.Success, "", playersStats);

            var playerEntries = playerNodes
                .Select(n => (Link: n.GetAttributeValue("href", ""), (string)null))
                .Where(p => !string.IsNullOrEmpty(p.Link))
                .DistinctBy(p => p.Link)
                .ToList();

            foreach (var (playerLink, _) in playerEntries)
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
                        _logger.LogInformation($"Рейтинг пользователя {nick} был спаршен успешно.");
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

                playersStats.Add(new PlayerStats
                {
                    Link = playerLink,
                    Name = nick,
                    City = string.IsNullOrEmpty(city) ? null : city,
                    Arm = arm,
                    Year = year == 0 ? null : year,
                    Rating = rating,
                    TournamentsPlayed = tournamentsPlayed,
                    WonGames = wonGames,
                    LostGames = lostGames
                });
            }

            return (MethodResult.Success, "", playersStats);
        }

        private static (int total, int won, int lost) ParseStats(HtmlNode cell)
        {
            int.TryParse(cell.GetAttributeValue("data-sort", "0"), out var total);
            var match = System.Text.RegularExpressions.Regex.Match(cell.InnerText, @"\((\d+)-(\d+)\)");
            int.TryParse(match.Groups[1].Value, out var won);
            int.TryParse(match.Groups[2].Value, out var lost);
            return (total, won, lost);
        }
    }
}
