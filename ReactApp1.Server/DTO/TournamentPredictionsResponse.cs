namespace ReactApp1.Server.DTO
{
    public class TournamentPredictionsResponse
    {
        public int PlayerCount { get; set; }
        public double AvgRating { get; set; }
        public int TotalRating { get; set; }
        public List<TournamentPredictionItem> Players { get; set; } = new();
    }
}
