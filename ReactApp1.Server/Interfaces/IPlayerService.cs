using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;

namespace ReactApp1.Server.Interfaces
{
    public interface IPlayerService
    {
        Task<(MethodResult, string, PlayerResponse)> GetPlayer(GetPlayerRequest request);
    }
}
