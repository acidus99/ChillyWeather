namespace Chilly.Clients;

using System;
using System.Net;

using Chilly.Models;
using Newtonsoft.Json.Linq;

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
            Name = ParseUtils.Cleanse(resp["cityName"]),
            State = ParseUtils.Cleanse(resp["regionName"]),
            Country = ParseUtils.Cleanse(resp["countryCode"]),
            Longitude = ParseUtils.GetCoordinate(resp["longitude"]),
            Latitude = ParseUtils.GetCoordinate(resp["latitude"])
        };
    }

    public GeoLocale GetCurrentLocale()
        => GetIPLocale("");   
}
