using System;
using System.Net;

using Chilly.Models;
using Newtonsoft.Json.Linq;

namespace Chilly.Clients
{
    /// <summary>
    /// Gets a locale via a free IP 2 location service
    /// </summary>
    public class FreeIpApiClient : ILocaleClient
    {
        WebClient client = new WebClient();
        public GeoLocale GetIPLocale(string ip)
        {
            var url = $"https://freeipapi.com/api/json/{ip}";

            var json = client.DownloadString(url);
            var resp = JObject.Parse(json);
            return new GeoLocale
            {
                Name = Cleanse(resp["cityName"]),
                State = Cleanse(resp["regionName"]),
                Country = Cleanse(resp["countryCode"]),
                Longitude = Coordinate(resp["longitude"]),
                Latitude = Coordinate(resp["latitude"])
            };
        }

        public GeoLocale GetCurrentLocale()
            => GetIPLocale("");

        private string Cleanse(JToken token)
            => token?.ToString() ?? "";

        private double Coordinate(JToken token)
            => (token != null) ? Convert.ToDouble(token.ToString()) : 0;
    }
}
