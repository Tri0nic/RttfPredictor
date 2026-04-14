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
        public async Task<IActionResult> PostTournamentsPlayersStats()
        {
            var (resultToday, messageToday, responseToday) = await _playersService.PostTodayTournamentsPlayersStats();

            var (resultFuture, messageFuture, responseFuture) = await _playersService.PostFutureTournamentsPlayersStats();

            return Ok($"За сегодня было обработано {responseToday.Count} турниров\nЗа ближайшие дни было обработано {responseFuture.Count} турниров");
        }
    }
}
