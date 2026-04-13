using Microsoft.EntityFrameworkCore;
using ReactApp1.Server.Data;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;
using ReactApp1.Server.Interfaces;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ReactApp1.Server.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly string _playersPath = @"D:\MyTestProjects\AspNetAndReact\ReactApp1\ReactApp1.Server\Persistence\players.json";
        private readonly string _tournamentPlayersStats = @"D:\MyTestProjects\AspNetAndReact\ReactApp1\ReactApp1.Server\Persistence\tournamentPlayersStats.json";

        private readonly AppDbContext _dbContext;

        public PlayerRepository(AppDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public async Task<(MethodResult, string, List<PlayerStats>?)> GetTournamentPlayersStats()
        {
            var entities = await _dbContext.PlayerStats.ToListAsync();

            // TODO: вынести через AutoMapper
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
                LostGames = e.LostGames
            }).ToList();

            return (MethodResult.Success, "", data);
        }

        public async Task<(MethodResult, string)> SaveNotStartedTournamentPlayersStats(List<PlayerStats> incoming)
        {
            try
            {
                // используем твой существующий метод как есть
                var (getResult, getError, existing) = await GetTournamentPlayersStats();

                List<PlayerStats> currentData;

                if (getResult == MethodResult.Success && existing != null)
                {
                    currentData = existing;
                }
                else
                {
                    // если файл не прочитался — считаем, что данных пока нет
                    currentData = new List<PlayerStats>();
                }

                var dict = currentData.ToDictionary(
                    x => (x.PlayerId, x.TournamentId),
                    x => x
                );

                foreach (var item in incoming)
                {
                    var key = (item.PlayerId, item.TournamentId);

                    if (dict.TryGetValue(key, out var existingItem))
                    {
                        UpdateAllFields(existingItem, item);
                    }
                    else
                    {
                        dict[key] = item;
                    }
                }

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(dict.Values.ToList(), options);
                await File.WriteAllTextAsync(_tournamentPlayersStats, json);

                return (MethodResult.Success, "");
            }
            catch (Exception ex)
            {
                return (MethodResult.InternalError, $"Ошибка сохранения: {ex.Message}");
            }
        }

        public async Task<(MethodResult, string)> SaveTournamentResults(List<PlayerStats> incoming)
        {
            try
            {
                // используем твой существующий метод как есть
                var (getResult, getError, existing) = await GetTournamentPlayersStats();

                List<PlayerStats> currentData;

                if (getResult == MethodResult.Success && existing != null)
                {
                    currentData = existing;
                }
                else
                {
                    // если файл не прочитался — считаем, что данных пока нет
                    currentData = new List<PlayerStats>();
                }

                var dict = currentData.ToDictionary(
                    x => (x.PlayerId, x.TournamentId),
                    x => x
                );

                foreach (var item in incoming)
                {
                    var key = (item.PlayerId, item.TournamentId);

                    if (dict.TryGetValue(key, out var existingItem))
                    {
                        UpdatePositionOnly(existingItem, item);
                    }
                    else
                    {
                        dict[key] = item;
                    }
                }

                var options = new JsonSerializerOptions
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(dict.Values.ToList(), options);
                await File.WriteAllTextAsync(_tournamentPlayersStats, json);

                return (MethodResult.Success, "");
            }
            catch (Exception ex)
            {
                return (MethodResult.InternalError, $"Ошибка сохранения: {ex.Message}");
            }
        }

        private static void UpdateAllFields(PlayerStats target, PlayerStats source)
        {
            var props = typeof(PlayerStats).GetProperties();

            foreach (var prop in props)
            {
                if (!prop.CanRead || !prop.CanWrite)
                    continue;

                if (prop.Name is nameof(PlayerStats.PlayerId) or nameof(PlayerStats.TournamentId))
                    continue;

                var value = prop.GetValue(source);
                prop.SetValue(target, value);
            }
        }

        private static void UpdatePositionOnly(PlayerStats target, PlayerStats source)
        {
            target.Position = source.Position;
        }
    }
}
