using System;

using Chilly.Clients;
using Chilly.Models;
namespace Chilly
{
    static class Program
    {
        static void Main(string[] args)
        {
            IWeatherClient client = ClientForge.ConfigureWeatherClient();

            string query = GetQueryFromArgs(args);

            var locations = client.LookupLocale(query);
            Forecast forecast = null;
            if(locations.Count > 0)
            {
                forecast = client.GetForecast(locations[0]);
                Renderer renderer = new Renderer(Console.Out);
                renderer.Render(forecast);
            }
            if (System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        static string GetQueryFromArgs(string [] args)
            => string.Join(" ", args);

    }
}
