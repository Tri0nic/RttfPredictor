using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Data;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Services
{
    public class TournamentService : ITournamentService
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public TournamentService(IDbContextFactory<AppDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public async Task<CountStatsDto> CountTournaments()
        {
            using var db = _dbContextFactory.CreateDbContext();
            var now = DateTime.UtcNow;
            var h24 = now.AddHours(-24);
            var d7 = now.AddDays(-7);

            var all = await db.Tournaments.CountAsync();
            var last24h = await db.Tournaments.Where(t => t.StartsAt >= h24).CountAsync();
            var last7d = await db.Tournaments.Where(t => t.StartsAt >= d7).CountAsync();

            return new CountStatsDto(all, last24h, last7d);
        }
    }
}
