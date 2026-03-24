using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<(MethodResult, string, PlayerResponse)> GetPlayer(int id)
        {
            var (result, message, response) = await _playerRepository.GetPlayer(id);

            return (result, message, response);
        }

        public async Task<(MethodResult, string, List<PlayerResponse>)> GetPlayers(GetPlayersRequest request)
        {
            var (result, message, response) = await _playerRepository.GetPlayers(request);

            return (result, message, response);
        }
    }
}
