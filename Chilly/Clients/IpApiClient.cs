namespace Chilly.Clients;

using System;
using CacheComms;
using Chilly.Models;
using Newtonsoft.Json.Linq;

/// <summary>
/// Gets a locale for an IP via http://ip-api.com/json/
/// </summary>
public class IpApiClient : ILocaleClient
{
    HttpRequestor Requestor = new HttpRequestor();

    public GeoLocale GetIPLocale(string ip)
    {
        var url = new Uri($"http://ip-api.com/json/{ip}");

        Requestor.GetAsString(url);
        var resp = JObject.Parse(Requestor.BodyText);
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
