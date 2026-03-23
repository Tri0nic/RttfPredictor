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

        public async Task<(MethodResult, string, PlayerResponse)> GetPlayer(GetPlayerRequest request)
        {
            // TODO: сделать парсер сайта по типу https://rttf.ru/tournaments/216872

            var (result, message, response) = await _playerRepository.GetPlayer(request);

            return (result, message, response);
        }
    }
}
