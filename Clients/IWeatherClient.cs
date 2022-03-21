using System;
using System.Collections.Generic;

using WeatherCat.Models;
namespace WeatherCat.Clients
{
    public interface IWeatherClient
    {
        List<GeoLocale> LookupLocale(string query);
        Forecast GetForecast(GeoLocale locale);
    }
}
