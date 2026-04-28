using ReactApp1.Server.DTO;
using ReactApp1.Server.Enums;
using ReactApp1.Server.Interfaces;

namespace ReactApp1.Server.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IMLModelRepository _mlModelRepository;
        private readonly IPredictionReadRepository _predictionRepository;

        public PredictionService(IPlayerRepository playerRepository, IMLModelRepository mlModelRepository, IPredictionReadRepository predictionRepository)
        {
            _playerRepository = playerRepository;
            _mlModelRepository = mlModelRepository;
            _predictionRepository = predictionRepository;
        }

        public async Task<(MethodResult, string, PlayerPredictionResult?)> GetPrediction(long playerId)
        {
            var latestStats = await _playerRepository.GetLatestPlayerStats(playerId);
            if (latestStats == null)
                return (MethodResult.NotFound, $"Игрок {playerId} не найден", null);

            var request = new PythonPredictRequest
            {
                PlayerId = latestStats.PlayerId,
                TournamentId = latestStats.TournamentId,
                Rating = latestStats.Rating,
                Year = latestStats.Year,
                Arm = latestStats.Arm,
                TournamentsPlayed = latestStats.TournamentsPlayed,
                WonGames = latestStats.WonGames,
                LostGames = latestStats.LostGames,
            };

            var (result, message, pythonResult) = await _mlModelRepository.PredictAsync(request);
            if (result != MethodResult.Success || pythonResult == null)
                return (result, message, null);

            await _predictionRepository.UpsertPrediction(
                latestStats.PlayerId, latestStats.TournamentId,
                pythonResult.PredictedPosition, pythonResult.ModelVersion);

            return (MethodResult.Success, "", new PlayerPredictionResult
            {
                PlayerId = pythonResult.PlayerId,
                PlayerName = latestStats.Name,
                TournamentId = pythonResult.TournamentId,
                PredictedPosition = pythonResult.PredictedPosition,
                PredictedAt = pythonResult.PredictedAt,
            });
        }

        public async Task<(MethodResult, string, TournamentPredictionsResponse?)> GetTournamentPredictions(long tournamentId)
        {
            var players = await _playerRepository.GetPlayersByTournamentId(tournamentId);
            if (players.Count == 0)
                return (MethodResult.NotFound, $"Турнир {tournamentId} не найден или нет игроков", null);

            var raw = new List<(PlayerStats Player, double Score, DateTime PredictedAt, string ModelVersion)>();

            var avgTournamentRating = players
                .Where(p => p.Rating.HasValue)
                .Select(p => (double)p.Rating!.Value)
                .DefaultIfEmpty(0)
                .Average();

            foreach (var player in players)
            {
                var request = new PythonPredictRequest
                {
                    PlayerId = player.PlayerId,
                    TournamentId = player.TournamentId,
                    Rating = player.Rating,
                    Year = player.Year,
                    Arm = player.Arm,
                    TournamentsPlayed = player.TournamentsPlayed,
                    WonGames = player.WonGames,
                    LostGames = player.LostGames,
                    AvgTournamentRating = avgTournamentRating > 0 ? avgTournamentRating : null,
                };

                var (result, _, pythonResult) = await _mlModelRepository.PredictAsync(request);
                if (result == MethodResult.Success && pythonResult != null)
                    raw.Add((player, pythonResult.PredictionScore, pythonResult.PredictedAt, pythonResult.ModelVersion));
            }

            var ranked = raw
                .OrderBy(x => x.Score)
                .Select((x, index) => new { x.Player, x.Score, x.PredictedAt, Rank = index + 1, x.ModelVersion })
                .ToList();

            foreach (var item in ranked)
                await _predictionRepository.UpsertPrediction(
                    item.Player.PlayerId, item.Player.TournamentId,
                    item.Rank, item.ModelVersion);

            var resultItems = ranked.Select(x => new TournamentPredictionItem
            {
                PlayerId = x.Player.PlayerId,
                PlayerName = x.Player.Name,
                Rating = x.Player.Rating,
                PredictedPosition = x.Rank,
                Score = x.Score,
                PredictedAt = x.PredictedAt,
            }).ToList();

            var withRating = players.Where(p => p.Rating.HasValue).ToList();
            var response = new TournamentPredictionsResponse
            {
                PlayerCount = resultItems.Count,
                AvgRating = withRating.Any() ? Math.Round(withRating.Average(p => (double)p.Rating!.Value), 1) : 0,
                TotalRating = withRating.Sum(p => p.Rating!.Value),
                Players = resultItems,
            };

            return (MethodResult.Success, "", response);
        }
    }
}
