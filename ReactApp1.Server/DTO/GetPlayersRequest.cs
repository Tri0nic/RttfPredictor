namespace ReactApp1.Server.DTO
{
    public class PostPlayersRequest
    {
        public int? Month { get; set; } = DateTime.Now.Month;

        public int? Year { get; set; } = DateTime.Now.Year;

        public string? Type { get; set; } = "all";

        public int? City { get; set; } = 77;

        public int? RatingMin { get; set; } = 0;

        public int? RatingMax { get; set; } = 1600;
    }
}
