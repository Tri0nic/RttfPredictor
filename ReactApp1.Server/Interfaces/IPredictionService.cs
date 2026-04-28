using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;

namespace ReactApp1.Server.Interfaces
{
    public interface IPredictionService
    {
        Task<(MethodResult, string, PlayerPredictionResult?)> GetPrediction(long playerId);
        Task<(MethodResult, string, TournamentPredictionsResponse?)> GetTournamentPredictions(long tournamentId);
    }
}
