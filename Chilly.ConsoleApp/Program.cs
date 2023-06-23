using System;

using System;
using Chilly.Clients;
using Chilly.Models;

namespace Chilly.ConsoleApp
{
    static class Program
    {
        static void Main(string[] args)
        {
            IWeatherClient client = ClientForge.ConfigureWeatherClient();

            string query = GetQueryFromArgs(args);

            GeoLocale locale = null;
            if (query.Length > 0)
            {
                var locations = client.LookupLocale(query);
                if (locations.Count > 0)
                {
                    //TODO: Display multiple locales to the user
                    locale = locations[0];
                }
                else
                {
                    Console.WriteLine($"No locations found for '{query}'");
                }
            }

            if (locale == null)
            {
                Console.WriteLine("Getting weather for current location");
                ILocaleClient localeClient = new FreeIpApiClient();
                locale = localeClient.GetCurrentLocale();
            }

            var forecast = client.GetForecast(locale);
            Renderer renderer = new Renderer(Console.Out);
            renderer.Render(forecast);

            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        static string GetQueryFromArgs(string[] args)
            => string.Join(" ", args);

    }
}
