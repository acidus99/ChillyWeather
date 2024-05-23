namespace Chilly.Clients;

using System;
using CacheComms;
using Chilly.Models;
using Newtonsoft.Json.Linq;

/// <summary>
/// Gets a locale via a free IP 2 location service
/// </summary>
public class FreeIpApiClient : ILocaleClient
{
    HttpRequestor Requestor = new HttpRequestor();
    public GeoLocale GetIPLocale(string ip)
    {
        var url = new Uri($"https://freeipapi.com/api/json/{ip}");
        Requestor.GetAsString(url);
        var resp = JObject.Parse(Requestor.BodyText);
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
