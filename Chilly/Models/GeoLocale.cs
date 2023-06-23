using Newtonsoft.Json;

namespace Chilly.Models
{
    public class GeoLocale
    {
        [JsonProperty("name")]
        public required string Name { get; set; }

        [JsonProperty("state")]
        public required string State { get; set; }

        [JsonProperty("country")]
        public required string Country { get; set; }

        [JsonProperty("lon")]
        public required double Longitude { get; set; }

        [JsonProperty("lat")]
        public required double Latitude { get; set; }
    }
}
