using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;

namespace ReactApp1.Server.Interfaces
{
    public interface IPlayerService
    {
        Task<(MethodResult, string, List<PlayerStats>)> GetTournamentPlayers();
        Task<(MethodResult, string, int)> PostPlayers(PostPlayersRequest request);
        Task<(MethodResult, string, List<PlayerStats>)> PostTournamentPlayersStats(string tournamentLink);
    }
}
