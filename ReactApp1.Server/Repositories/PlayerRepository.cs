using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;
using ReactApp1.Server.Interfaces;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace ReactApp1.Server.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private const string _playersPath = @"D:\MyTestProjects\AspNetAndReact\ReactApp1\ReactApp1.Server\Persistence\players.json";
        private const string _playersAfterTournamentsPath = @"D:\MyTestProjects\AspNetAndReact\ReactApp1\ReactApp1.Server\Persistence\playersAfterTournaments.json";

        public async Task<(MethodResult, string, List<PlayerResponse>)> GetPlayers(GetPlayersRequest request)
        {
            // Когда будет БД сделать отложенное выполнение
            var json = File.ReadAllText(_playersPath);
            var players = JsonSerializer.Deserialize<List<PlayerResponse>>(json);

            if (request.Id.HasValue)
            {
                players = players.Where(x => x.Id == request.Id).ToList();
            }

            if (request.Name != null)
            {
                players = players.Where(x => x.Name.Contains(request.Name)).ToList();
            }

            if (request.Rating.HasValue)
            {
                players = players.Where(x => x.Rating == request.Rating).ToList();
            }

            return (MethodResult.Success, null, players);
        }

        public async Task<(MethodResult, string, PlayerResponse)> GetPlayer(int id)
        {
            var json = File.ReadAllText(_playersPath);
            var players = JsonSerializer.Deserialize<List<PlayerResponse>>(json);

            var player = players.Where(x => x.Id == id).FirstOrDefault();

            if (player == null)
            {
                return (MethodResult.NotFound, "Игрок не был найден", null);
            }

            return (MethodResult.Success, null, player);
        }


        public async Task<(MethodResult, string)> SavePlayersAfterTournaments(List<PlayerAfterTournament> playersAfterTournaments)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(playersAfterTournaments, options);
            await File.WriteAllTextAsync(_playersAfterTournamentsPath, json);

            return (MethodResult.Success, "");
        }
    }
}
