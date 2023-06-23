namespace Chilly.Clients;

using System.Collections.Generic;

using Chilly.Models;

public interface IWeatherClient
{
    List<GeoLocale> LookupLocale(string query);
    Forecast GetForecast(GeoLocale locale, bool useMetric=false);
}
