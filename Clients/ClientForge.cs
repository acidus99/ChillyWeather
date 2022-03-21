using System;
using System.IO;

using IniParser;
using IniParser.Model;
namespace Chilly.Clients
{
    public static class ClientForge
    {
        public static IWeatherClient ConfigureWeatherClient(string configFile = "")
        {
            if(String.IsNullOrEmpty(configFile))
            {
                configFile = GetDefaultConfigFile();
            }

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
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error loading API key from '{configFile}'.");
                Environment.Exit(1);
            }
            return null;
        }

        private static string GetDefaultConfigFile()
            => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.chilly";

    }
}
