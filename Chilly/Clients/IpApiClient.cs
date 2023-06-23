namespace Chilly.Clients;

using System;
using System.Net;

using Chilly.Models;
using Newtonsoft.Json.Linq;

/// <summary>
/// Gets a locale for an IP via http://ip-api.com/json/
/// </summary>
public class IpApiClient : ILocaleClient
{
    WebClient client = new WebClient();
    public GeoLocale GetIPLocale(string ip)
    {
        var url = $"http://ip-api.com/json/{ip}";

        var json = client.DownloadString(url);
        var resp = JObject.Parse(json);
        return new GeoLocale
        {
            Name = Cleanse(resp["city"]),
            State = Cleanse(resp["regionName"]),
            Country = Cleanse(resp["countryCode"]),
            Longitude = Coordinate(resp["lon"]),
            Latitude = Coordinate(resp["lat"])
        };
    }

    public GeoLocale GetCurrentLocale()
        => GetIPLocale("");

    private string Cleanse(JToken token)
        => token?.ToString() ?? "";

    private double Coordinate(JToken token)
        => (token != null) ? Convert.ToDouble(token.ToString()) : 0;
}
