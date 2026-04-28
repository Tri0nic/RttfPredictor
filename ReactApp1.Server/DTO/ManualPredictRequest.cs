namespace ReactApp1.Server.DTO
{
    public record ManualPredictRequest(
        int? Rating,
        int? Year,
        string? Arm,
        int? TournamentsPlayed,
        int? WonGames,
        int? LostGames,
        double? AvgTournamentRating);
}
