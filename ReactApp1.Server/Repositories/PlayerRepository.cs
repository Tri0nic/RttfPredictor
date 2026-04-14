using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Data;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Entities;
using ReactApp1.Server.Enums;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;
        private readonly ILogger<PlayerRepository> _logger;

        public PlayerRepository(IDbContextFactory<AppDbContext> dbContextFactory, ILogger<PlayerRepository> logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        public async Task<(MethodResult, string, List<PlayerStats>?)> GetTournamentPlayersStats()
        {
            using var context = _dbContextFactory.CreateDbContext();

            var entities = await context.PlayerStats
                .Include(e => e.Tournament)
                .ToListAsync();

            var data = entities.Select(e => new PlayerStats()
            {
                PlayerId = e.PlayerId,
                TournamentId = e.TournamentId,
                Position = e.Position,
                Name = e.Name,
                City = e.City,
                Year = e.Year,
                Arm = e.Arm,
                Rating = e.Rating,
                TournamentsPlayed = e.TournamentsPlayed,
                WonGames = e.WonGames,
                LostGames = e.LostGames,
                TournamentDate = e.Tournament.StartsAt
            }).ToList();

            return (MethodResult.Success, "", data);
        }

        public async Task<bool> TournamentExists(long tournamentId)
        {
            using var context = _dbContextFactory.CreateDbContext();
            return await context.Tournaments.AnyAsync(t => t.Id == tournamentId);
        }

        public async Task<(MethodResult, string)> UpsertTournament(long tournamentId, DateTime? startsAt)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var existing = await context.Tournaments.FindAsync(tournamentId);
            if (existing == null)
            {
                context.Tournaments.Add(new TournamentEntity
                {
                    Id = tournamentId,
                    StartsAt = startsAt
                });
                _logger.LogInformation($"Турнир {tournamentId} добавлен в БД");
            }
            else
            {
                existing.StartsAt = startsAt;
                _logger.LogInformation($"Турнир {tournamentId} обновлён в БД");
            }

            await context.SaveChangesAsync();
            return (MethodResult.Success, "");
        }

        public async Task<(MethodResult, string)> SaveNotStartedTournamentPlayersStats(List<PlayerStats> incoming)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var playerIds = incoming.Select(i => i.PlayerId).ToList();

            var existing = await context.PlayerStats
                .Where(e => playerIds.Contains(e.PlayerId) && e.TournamentId == incoming.First().TournamentId)
                .ToListAsync();

            var existingDict = existing.ToDictionary(e => (e.PlayerId, e.TournamentId));

            var i = 0;
            foreach (var item in incoming)
            {
                

                if (existingDict.TryGetValue((item.PlayerId, item.TournamentId), out var entity))
                {
                    entity.Name = item.Name;
                    entity.City = item.City;
                    entity.Year = item.Year;
                    entity.Arm = item.Arm;
                    entity.Rating = item.Rating;
                    entity.TournamentsPlayed = item.TournamentsPlayed;
                    entity.WonGames = item.WonGames;
                    entity.LostGames = item.LostGames;
                    entity.UpdatedAt = DateTime.UtcNow;

                    _logger.LogInformation($"Игрок обновлен в БД {item.Name} -- {++i}/{incoming.Count}");
                }
                else
                {
                    context.PlayerStats.Add(new PlayerStatsEntity
                    {
                        PlayerId = item.PlayerId,
                        TournamentId = item.TournamentId,
                        Name = item.Name,
                        City = item.City,
                        Year = item.Year,
                        Arm = item.Arm,
                        Rating = item.Rating,
                        TournamentsPlayed = item.TournamentsPlayed,
                        WonGames = item.WonGames,
                        LostGames = item.LostGames,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });

                    _logger.LogInformation($"Игрок добавлен в БД {item.Name} -- {++i}/{incoming.Count}");
                }
            }

            await context.SaveChangesAsync();
            return (MethodResult.Success, "");
        }

        public async Task<(MethodResult, string)> SaveTournamentResults(List<PlayerStats> incoming)
        {
            using var context = _dbContextFactory.CreateDbContext();

            var playerIds = incoming.Select(i => i.PlayerId).ToList();
            var tournamentIds = incoming.Select(i => i.TournamentId).ToList();

            var existing = await context.PlayerStats
                .Where(e => playerIds.Contains(e.PlayerId) && tournamentIds.Contains(e.TournamentId))
                .ToListAsync();

            var existingDict = existing.ToDictionary(e => (e.PlayerId, e.TournamentId));

            var i = 0;
            foreach (var item in incoming)
            {
                if (existingDict.TryGetValue((item.PlayerId, item.TournamentId), out var entity))
                {
                    entity.Position = item.Position;
                    entity.UpdatedAt = DateTime.UtcNow;

                    _logger.LogInformation($"Место игрока добавлено в БД {item.Name} -- {++i}/{incoming.Count}");
                }
                else
                {
                    _logger.LogInformation($"По игроку нет данных в БД до начала турнира. Сохранение результатов невозможно. {item.Name} -- {++i}/{incoming.Count}");
                }
            }

            await context.SaveChangesAsync();
            return (MethodResult.Success, "");
        }
    }
}
