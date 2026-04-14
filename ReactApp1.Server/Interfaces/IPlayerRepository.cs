using Microsoft.AspNetCore.Mvc;
using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;

namespace ReactApp1.Server.Interfaces
{
    public interface IPlayerRepository
    {
        Task<(MethodResult, string, List<PlayerStats>)> GetTournamentPlayersStats();
        Task<bool> TournamentExists(long tournamentId);
        Task<(MethodResult, string)> UpsertTournament(long tournamentId, DateTime? startsAt);
        Task<(MethodResult, string)> SaveNotStartedTournamentPlayersStats(List<PlayerStats> playersAfterTournaments);
        Task<(MethodResult, string)> SaveTournamentResults(List<PlayerStats> playersAfterTournaments);
    }
}
