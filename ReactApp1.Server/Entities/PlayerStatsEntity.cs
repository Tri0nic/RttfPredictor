namespace ReactApp1.Server.Entities
{
    public class PlayerStatsEntity
    {
        public required string PlayerId { get; set; }
        public required string TournamentId { get; set; }
        public int? Position { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public int? Year { get; set; }
        public string? Arm { get; set; }
        public int? Rating { get; set; }
        public int? TournamentsPlayed { get; set; }
        public int? WonGames { get; set; }
        public int? LostGames { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
