using System;
using System.IO;

using WeatherCat.Clients;
using WeatherCat.Models;

using IniParser;
using IniParser.Model;


namespace WeatherCat
{
    static class Program
    {

        static void Main(string[] args)
        {
            IWeatherClient client = ConfigureClient();
            var locations = client.LookupLocale("Atlanta GA");
            Forecast forecast = null;
            if(locations.Count > 0)
            {
                forecast = client.GetForecast(locations[0]);
                Renderer renderer = new Renderer(Console.Out);
                renderer.Render(forecast);
            }

            //display foremat here
            int x = 4;

        }

        private static IWeatherClient ConfigureClient()
        {
            string configFile = GetConfigFile();
            if (!File.Exists(configFile))
            {
                Console.Error.WriteLine($"Could not locate config file '{configFile}'");
                Environment.Exit(1);
            }
            try
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(configFile);
                var apiKey = data["api"]["OpenWeatherKey"];
                return new OpenWeatherClient(apiKey);
            } catch(Exception ex)
            {
                Console.Error.WriteLine($"Error loading API key from '{configFile}'.");
                Environment.Exit(1);
            }
            return null;
        }

        private static string GetConfigFile()
            => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.chilly";
    }
}
