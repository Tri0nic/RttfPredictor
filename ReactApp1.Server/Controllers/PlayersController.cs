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

        [HttpGet]
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


        #region Methods needs rework
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostPlayersRequest request)
        {
            // Парсит турниры по календарю - в будущем нужно будет доработать, пока что не использовать
            var (result, message, response) = await _playersService.PostPlayers(request);

            return Ok($"Было обработано {response} игроков");
        }

        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
        #endregion
    }
}
