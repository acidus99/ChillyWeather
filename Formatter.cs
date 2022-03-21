﻿using System;
using System.Globalization;

using Chilly.Models;

namespace Chilly
{
    //reusable formatter for rendering forecast data
    public class Formatter
    {
        CultureInfo culture;

        public Formatter(string cultureName = "en-US")
        {
            culture = new CultureInfo(cultureName, false);
            //made to change if using celsius
            culture.NumberFormat.PercentDecimalDigits = 0;
        }

        public string FormatDay(DateTime curr, DateTime next)
        {
            if (curr.AddDays(1) > next)
            {
                return "Tomorrow";
            }
            return next.ToString("ddd d");
        }

        public string FormatTime(DateTime time)
            => time.ToString("ddd d h:mm tt");

        public string FormatChance(float chance)
            => chance.ToString("P", culture.NumberFormat);

        public string FormatTemp(float temp)
            => Math.Round(temp).ToString() + "°";

        public string EmojiForCurrentWeather(CurrentCondition current)
        {
            //is it night or day?
            bool IsDay = (current.Time > current.Sunrise && current.Time < current.Sunset);
            if (IsDay)
            {
                return EmojiForWeather(current.Weather.Type);
            }
            //if the emoji doesn't have a sun, pass it through. overwise flip it to a moon
            //TODO: change the moon emoji based on the moon phase
            switch (current.Weather.Type)
            {
                case WeatherType.Rain:
                case WeatherType.Snow:
                case WeatherType.Thunderstorm:
                    return EmojiForWeather(current.Weather.Type);
                default:
                    return "🌕";
            }
        }

        public string EmojiForWeather(WeatherType weather)
        {
            switch (weather)
            {
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

        public string FormatDescription(Weather weather)
            => culture.TextInfo.ToTitleCase(weather.Description);
    }
}