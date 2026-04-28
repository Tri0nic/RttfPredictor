using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModelController : ControllerBase
    {
        private readonly IMLModelRepository _mlModelRepository;

        public ModelController(IMLModelRepository mlModelRepository)
        {
            _mlModelRepository = mlModelRepository;
        }

        [HttpGet("feature-importance")]
        public async Task<IActionResult> GetFeatureImportance()
        {
            var (result, message, data) = await _mlModelRepository.GetFeatureImportanceAsync();
            return result == MethodResult.Success ? Ok(data) : StatusCode(503, new { message });
        }

        [HttpPost("predict-score")]
        public async Task<IActionResult> PredictScore([FromBody] ManualPredictRequest request)
        {
            var pythonRequest = new PythonPredictRequest
            {
                PlayerId = 0,
                TournamentId = 0,
                Rating = request.Rating,
                Year = request.Year,
                Arm = request.Arm,
                TournamentsPlayed = request.TournamentsPlayed,
                WonGames = request.WonGames,
                LostGames = request.LostGames,
                AvgTournamentRating = request.AvgTournamentRating,
            };
            var (result, message, data) = await _mlModelRepository.PredictAsync(pythonRequest);
            return result == MethodResult.Success
                ? Ok(new { score = data!.PredictionScore })
                : StatusCode(503, new { message });
        }
    }
}
