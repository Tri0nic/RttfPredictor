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
        public async Task<IEnumerable<PlayerResponse>> GetMany([FromQuery] GetPlayersRequest request)
        {
            var (result, message, response) = await _playersService.GetPlayers(request);

            return response;
        }

        [HttpGet("{id:int:min(1)}")]
        public async Task<PlayerResponse> Get(int id)
        {
            var (result, message, response) = await _playersService.GetPlayer(id);

            return response;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostPlayersRequest request)
        {
            var (result, message, response) = await _playersService.PostPlayers(request);

            return Ok($"Было обработано {response} игроков");
        }

        [HttpPost("PostTournamentPlayersStats")]
        public async Task<IActionResult> Post([FromBody] string tournamentLink)
        {
            var (result, message, response) = await _playersService.PostTournamentPlayersStats(tournamentLink);

            return Ok($"Было обработано {response.Count} игроков");
        }

        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
