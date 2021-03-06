using System;
using Chilly.Models;

namespace Chilly.Clients
{
    public interface ILocaleClient
    {
        GeoLocale GetCurrentLocale();
        GeoLocale GetIPLocale(string ip);
    }
}
