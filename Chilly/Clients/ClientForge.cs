namespace Chilly.Clients;

using System;
using System.IO;

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
            string contents = File.ReadAllText(configFile);
            if(contents.StartsWith("OpenWeatherKey="))
            {
                var apiKey = contents.Substring("OpenWeatherKey=".Length);
                return new OpenWeatherClient(apiKey);
            }
            throw new ApplicationException();
           
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
