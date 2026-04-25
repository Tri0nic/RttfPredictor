using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.Enums;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Controllers
{
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly IPredictionService _predictionService;

        public PredictionController(IPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpGet("api/players/{id}/prediction")]
        public async Task<IActionResult> GetPlayerPrediction(long id)
        {
            var (result, message, data) = await _predictionService.GetPrediction(id);

            return result switch
            {
                MethodResult.Success => Ok(data),
                MethodResult.NotFound => NotFound(new { message }),
                _ => StatusCode(500, new { message })
            };
        }

        [HttpGet("api/tournaments/{id}/predictions")]
        public async Task<IActionResult> GetTournamentPredictions(long id)
        {
            var (result, message, data) = await _predictionService.GetTournamentPredictions(id);

            return result switch
            {
                MethodResult.Success => Ok(data),
                MethodResult.NotFound => NotFound(new { message }),
                _ => StatusCode(500, new { message })
            };
        }
    }
}
