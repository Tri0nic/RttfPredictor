using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;

namespace ReactApp1.Server.Interfaces
{
    public interface IPlayerRepository
    {
        Task<(MethodResult, string, List<PlayerResponse>)> GetPlayers(GetPlayersRequest request);
        Task<(MethodResult, string, PlayerResponse)> GetPlayer(int id);
    }
}
