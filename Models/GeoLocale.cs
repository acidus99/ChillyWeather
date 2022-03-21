using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace WeatherCat.Models
{
    public class GeoLocale
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("lon")]
        public double Longitude { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }
}
