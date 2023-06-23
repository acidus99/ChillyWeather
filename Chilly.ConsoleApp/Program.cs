namespace Chilly.ConsoleApp;

using System;
using Chilly.Clients;
using Chilly.Models;

static class Program
{
    static void Main(string[] args)
    {
        IWeatherClient client = ClientForge.ConfigureWeatherClient();

        GeoLocale locale = GetLocale(client, args);

        var forecast = client.GetForecast(locale);

        var renderer = new ConsoleRenderer(Console.Out);
        renderer.Render(forecast);

        if (System.Diagnostics.Debugger.IsAttached)
            System.Diagnostics.Debugger.Break();
    }

    static GeoLocale GetLocale(IWeatherClient client, string[] args)
    {
        var query = string.Join(" ", args);
        if (query.Length > 0)
        {
            var locations = client.LookupLocale(query);
            if (locations.Count > 0)
            {
                return locations[0];
            }
        }
        Console.WriteLine($"No locations found for '{query}'.");
        Console.WriteLine("Using IP Address for current location");
        
        ILocaleClient localeClient = new FreeIpApiClient();
        return localeClient.GetCurrentLocale();
    }
}
