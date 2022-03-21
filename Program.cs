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
            var locations = client.LookupLocale("Atlanta GA");
            Forecast forecast = null;
            if(locations.Count > 0)
            {
                forecast = client.GetForecast(locations[0]);
                Renderer renderer = new Renderer(Console.Out);
                renderer.Render(forecast);
            }
        }
    }
}
