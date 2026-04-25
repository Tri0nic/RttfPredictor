using ReactApp1.Server.DTO;

namespace ReactApp1.Server.Interfaces
{
    public interface IPredictionReadRepository
    {
        Task<List<TournamentPredictionItem>> GetTournamentPredictions(long tournamentId);
        Task UpsertPrediction(long playerId, long tournamentId, int predictedPosition, string modelVersion);
    }
}
