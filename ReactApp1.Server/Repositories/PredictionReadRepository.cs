using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Data;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Entities;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Repositories
{
    public class PredictionReadRepository : IPredictionReadRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public PredictionReadRepository(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<List<TournamentPredictionItem>> GetTournamentPredictions(long tournamentId)
        {
            using var context = _dbContextFactory.CreateDbContext();

            return await context.Predictions
                .Where(p => p.TournamentId == tournamentId)
                .Join(
                    context.PlayerStats.Where(ps => ps.TournamentId == tournamentId),
                    p => p.PlayerId,
                    ps => ps.PlayerId,
                    (p, ps) => new TournamentPredictionItem
                    {
                        PlayerId = p.PlayerId,
                        PlayerName = ps.Name,
                        Rating = ps.Rating,
                        PredictedPosition = p.PredictedPosition,
                        PredictedAt = p.CreatedAt,
                    })
                .OrderBy(x => x.PredictedPosition)
                .ToListAsync();
        }

        public async Task UpsertPrediction(long playerId, long tournamentId, int predictedPosition, string modelVersion)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var existing = await context.Predictions
                .FirstOrDefaultAsync(p => p.PlayerId == playerId && p.TournamentId == tournamentId);

            if (existing != null)
            {
                existing.PredictedPosition = predictedPosition;
                existing.ModelVersion = modelVersion;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                context.Predictions.Add(new PredictionEntity
                {
                    PlayerId = playerId,
                    TournamentId = tournamentId,
                    PredictedPosition = predictedPosition,
                    ModelVersion = modelVersion,
                    CreatedAt = DateTime.UtcNow,
                });
            }

            await context.SaveChangesAsync();
        }
    }
}
