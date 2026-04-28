using ReactApp1.Server.DTO;

namespace ReactApp1.Server.Interfaces
{
    public interface ITournamentService
    {
        Task<CountStatsDto> CountTournaments();
    }
}
