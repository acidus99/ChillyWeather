namespace Chilly.Clients;

using Chilly.Models;

public interface ILocaleClient
{
    GeoLocale GetCurrentLocale();
    GeoLocale GetIPLocale(string ip);
}
