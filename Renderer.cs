using System;
using System.Globalization;
using System.IO;

using Chilly.Models;

namespace Chilly
{
    public class Renderer
    {
        TextWriter Fout;

        public Renderer(TextWriter fout)
        {
            Fout = fout;
        }

        public void Render(Forecast forecast)
        {
            Fout.WriteLine($"Weather for {forecast.Location.Name} @ {forecast.Current.Time.ToString("ddd d H:mm")}");
            Fout.WriteLine($"{EmojiForCurrentWeather(forecast.Current)} {RenderTemp(forecast.Current.Temp)}F");
            Fout.WriteLine($"{FormatDescription(forecast.Current.Weather)}");

            Fout.WriteLine();
            foreach(DailyCondition daily in forecast.Daily)
            {
                if(daily.Time < forecast.Current.Time)
                {
                    continue;
                }
                Fout.WriteLine($"{FormatDay(forecast.Current.Time, daily.Time)}: {EmojiForWeather(daily.Weather.Type)} {RenderTemp(daily.LowTemp)} to {RenderTemp(daily.HighTemp)}");
                if (daily.ChanceOfPrecipitation == 0)
                {
                    Fout.WriteLine(FormatDescription(daily.Weather));
                }
                else
                {
                    Fout.WriteLine($"💧 {FormatChance(daily.ChanceOfPrecipitation)} - {FormatDescription(daily.Weather)}");
                }
                Fout.WriteLine();
            }

        }

        private string FormatDay(DateTime curr, DateTime next)
        {
            if(curr.AddDays(1) > next)
            {
                return "Tomorrow";
            }
            return next.ToString("ddd d");
        }

        private string FormatChance(float chance)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            nfi.PercentDecimalDigits = 0;
            return chance.ToString("P", nfi);
        } 

        private string RenderTemp(float temp)
            => Math.Round(temp).ToString() + "°";

        private string EmojiForCurrentWeather(CurrentCondition current)
        {
            //is it night or day?
            bool IsDay = (current.Time > current.Sunrise && current.Time < current.Sunset);
            if(IsDay)
            {
                return EmojiForWeather(current.Weather.Type);
            }
            //we need to switch the emoji that have sun
            switch(current.Weather.Type)
            {
                case WeatherType.Rain:
                case WeatherType.Snow:
                case WeatherType.Thunderstorm:
                    return EmojiForWeather(current.Weather.Type);
                default:
                    return "🌕";
            }
        }

        private string EmojiForWeather(WeatherType weather)
        {
            switch(weather) {
                case WeatherType.ScatteredClouds:
                    return "🌤";
                case WeatherType.PartlyCloudy:
                    return "⛅️";
                case WeatherType.Cloudy:
                    return "🌥";
                case WeatherType.Overcast:
                    return "☁️";
                case WeatherType.Rain:
                    return "☔️";
                case WeatherType.Snow:
                    return "❄️";
                case WeatherType.Thunderstorm:
                    return "⛈";
                default:
                    //clear, so return a sun or moon based on 
                    return "☀️";
            }
        }

        private string FormatDescription(Weather weather)
        {
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo ti = new CultureInfo("en-US", false).TextInfo;
            return ti.ToTitleCase(weather.Description);
        }

    }
}
