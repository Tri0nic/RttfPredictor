using System.Text.Json.Serialization;

namespace ReactApp1.Server.DTO
{
    public class PostTournamentsPlayersStatsRequest
    {
        [JsonPropertyName("start_day")]
        public int startDay { get; set; } = -1;

        [JsonPropertyName("end_day")]
        public int endDay { get; set; } = 2;
    }
}
