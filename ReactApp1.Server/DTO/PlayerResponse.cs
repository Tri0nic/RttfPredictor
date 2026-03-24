using System.Text.Json.Serialization;

namespace ReactApp1.Server.DTO
{
    public class PlayerResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }
    }
}
