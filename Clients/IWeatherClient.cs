using System;
using System.Collections.Generic;

using Chilly.Models;
namespace Chilly.Clients
{
    public interface IWeatherClient
    {
        List<GeoLocale> LookupLocale(string query);
        Forecast GetForecast(GeoLocale locale);
    }
}
