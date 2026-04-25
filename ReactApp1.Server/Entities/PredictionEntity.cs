namespace ReactApp1.Server.Entities
{
    public class PredictionEntity
    {
        public long Id { get; set; }
        public long PlayerId { get; set; }
        public long TournamentId { get; set; }
        public int PredictedPosition { get; set; }
        public string ModelVersion { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
