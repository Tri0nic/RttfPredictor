using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentsController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;

        public TournamentsController(ITournamentService tournamentService)
        {
            _tournamentService = tournamentService;
        }

        [HttpGet("count")]
        public async Task<IActionResult> CountTournaments()
        {
            return Ok(await _tournamentService.CountTournaments());
        }
    }
}
