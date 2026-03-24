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

        public PlayerService(IPlayerRepository playerRepository, IOptions<RttfLinks> rttfLinks)
        {
            _playerRepository = playerRepository;
            _rttfLinks = rttfLinks.Value;
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

        public async Task<(MethodResult, string, List<PlayerResponse>)> PostPlayers(PostPlayersRequest request)
        {
            var (resultParseTournaments, messageParseTournaments, tournamentLinks) = await ParseTournaments(request);

            var (resultGetPlayersFromTournaments, messageGetPlayersFromTournaments, playersAfterTournaments) = await GetPlayersFromTournaments(tournamentLinks);


            var (result, message) = await _playerRepository.SavePlayersAfterTournaments(playersAfterTournaments);

            return (MethodResult.Success, "", new List<PlayerResponse>());
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
            var result = new List<PlayerAfterTournament>();

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
                    if (cells == null || cells.Count < 8) continue;//

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

                    result.Add(new PlayerAfterTournament
                    {
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

            return (MethodResult.Success, "", result);
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
