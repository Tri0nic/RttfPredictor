using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;

namespace ReactApp1.Server.Interfaces
{
    public interface IPlayerService
    {
        Task<(MethodResult, string, List<PlayerStats>)> GetTournamentPlayers();
        Task<(MethodResult, string, List<PlayerStats>)> PostTournamentPlayersStats(string tournamentLink);
        Task<(MethodResult, string, Dictionary<long, List<PlayerStats>>)> PostTodayTournamentsPlayersStats();
        Task<(MethodResult, string, Dictionary<long, List<PlayerStats>>)> PostFutureTournamentsPlayersStats();
    }
}
