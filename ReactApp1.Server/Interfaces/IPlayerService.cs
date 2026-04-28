using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;

namespace ReactApp1.Server.Interfaces
{
    public interface IPlayerService
    {
        Task<int> CountPlayers();
        Task<(MethodResult, string, List<PlayerStats>)> GetTournamentPlayers();
        Task<(MethodResult, string, List<PlayerStats>)> PostTournamentPlayersStats(string tournamentLink);
        Task<(MethodResult, string, Dictionary<string, Dictionary<long, List<PlayerStats>>>)> PostTournamentsPlayersStatsNearbyDays(int startDay, int endDay);
    }
}
