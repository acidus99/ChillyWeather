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
            Name = ParseUtils.Cleanse(resp["city"]),
            State = ParseUtils.Cleanse(resp["regionName"]),
            Country = ParseUtils.Cleanse(resp["countryCode"]),
            Longitude = ParseUtils.GetCoordinate(resp["lon"]),
            Latitude = ParseUtils.GetCoordinate(resp["lat"])
        };
    }

    public GeoLocale GetCurrentLocale()
        => GetIPLocale("");
}
