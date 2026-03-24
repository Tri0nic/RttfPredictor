using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;
using ReactApp1.Server.Interfaces;
using System.Text.Json;

namespace ReactApp1.Server.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private const string _playersPath = @"D:\MyTestProjects\AspNetAndReact\ReactApp1\ReactApp1.Server\Persistence\players.json";
        //private readonly string _playersPath;

        //public PlayerRepository(IOption playersPath)
        //{
        //    _playersPath = playersPath;
        //}

        public async Task<(MethodResult, string, List<PlayerResponse>)> GetPlayers(GetPlayersRequest request)
        {
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


        //Когда будет БД сделать отложенное выполнение по типу такого:
        //public async Task<IEnumerable<CurrentIncome>> GetByDateAsync(GetCurrentIncomeRequest request)
        //{
        //    await using var dbContext = _dbContextFactory.Create();

        //    var query = dbContext.CurrentIncomes.AsQueryable();

        //    if (request.IsActive.HasValue)
        //    {
        //        query = query.Where(x => x.IsActive == request.IsActive.Value);
        //    }
        //    else
        //    {
        //        query = query.Where(x => x.IsActive);
        //    }

        //    if (request.IcId.HasValue)
        //    {
        //        query = query.Where(x => x.IcId == request.IcId.Value);
        //    }

        //    if (request.StartDate.HasValue)
        //    {
        //        var start = request.StartDate.Value.Date;
        //        var end = request.EndDate?.Date.AddDays(1) ?? start.AddDays(1);

        //        query = query.Where(x =>
        //            x.IncomeDate >= start &&
        //            x.IncomeDate < end);
        //    }

        //    if (request.IsConsiderLimit.HasValue)
        //    {
        //        query = query.Where(x => x.IsConsiderLimit == request.IsConsiderLimit.Value);
        //    }

        //    return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        //}
    }
}
