using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService _playersService;

        public PlayersController(IPlayerService playersService)
        {
            _playersService = playersService;
        }

        [HttpGet("AllPlayers")]
        public async Task<IEnumerable<PlayerStats>> GetTournamentPlayers()
        {
            var (result, message, response) = await _playersService.GetTournamentPlayers();

            return response;
        }

        [HttpPost("PostTournamentPlayersStats")]
        public async Task<IActionResult> PostTournamentPlayersStats([FromBody] string tournamentLink)
        {
            var (result, message, response) = await _playersService.PostTournamentPlayersStats(tournamentLink);

            return Ok($"Для турнира {tournamentLink} было обработано {response.Count} игроков");
        }

        [HttpPost("PostTournamentsPlayersStats")]
        public async Task<IActionResult> PostTournamentsPlayersStats([FromBody] PostTournamentsPlayersStatsRequest request)
        {
            var (result, message, response) = await _playersService.PostTournamentsPlayersStatsNearbyDays(request.startDay, request.endDay);

            var lines = new List<string>();
            var totalTournaments = 0;
            var totalPlayers = 0;

            foreach (var (date, tournaments) in response)
            {
                var dayTournaments = tournaments.Count;
                var dayPlayers = tournaments.Values.Sum(p => p.Count);
                totalTournaments += dayTournaments;
                totalPlayers += dayPlayers;

                lines.Add($"За {date} было обработано {dayTournaments} турниров и {dayPlayers} игроков");
            }

            var startDate = DateTime.Now.Date.AddDays(request.startDay).ToString("dd.MM.yyyy");
            var endDate = DateTime.Now.Date.AddDays(request.endDay).ToString("dd.MM.yyyy");
            lines.Add($"\nИтого за {startDate} - {endDate}: {totalTournaments} турниров и {totalPlayers} игроков");

            return Ok(string.Join("\n", lines));
        }
    }
}
