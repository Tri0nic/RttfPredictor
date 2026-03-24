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
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
