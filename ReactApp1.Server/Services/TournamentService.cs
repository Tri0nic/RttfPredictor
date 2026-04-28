using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Data;
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

        public async Task<int> CountTournaments()
        {
            using var db = _dbContextFactory.CreateDbContext();
            return await db.Tournaments.CountAsync();
        }
    }
}
