namespace Chilly;

using System;
using System.Globalization;

using Chilly.Models;

//reusable formatter for rendering forecast data
public class Formatter
{
    CultureInfo culture;

    public bool IsMetric;

    public Formatter(string cultureName = "en-US")
    {
        culture = new CultureInfo(cultureName, false);
        //made to change if using celsius
        culture.NumberFormat.PercentDecimalDigits = 0;
    }

    public string FormatDay(DateTime curr, DateTime next)
    {
        if (curr.Date.AddDays(1) == next.Date)
        {
            return "Tomorrow";
        }
        return next.ToString("ddd d") + GetDaySuffix(next.Day);
    }

    string GetDaySuffix(int day)
    {
        switch (day)
        {
            case 1:
            case 21:
            case 31:
                return "st";
            case 2:
            case 22:
                return "nd";
            case 3:
            case 23:
                return "rd";
            default:
                return "th";
        }
    }

    public string FormatHour(DateTime time)
        => (IsMetric) ? time.ToString("H:00") : time.ToString("h tt");

    public string FormatDayTime(DateTime time)
        => (IsMetric) ? time.ToString("ddd d H:mm") : time.ToString("ddd d h:mm tt");

    public string FormatTime(DateTime time)
        => (IsMetric) ? time.ToString("H:mm") : time.ToString("h:mm tt");

    public string FormatChance(float chance)
        => chance.ToString("P", culture.NumberFormat);

    public string FormatTemp(float temp)
        => Math.Round(temp).ToString() + DegreeUnit;

    public string DegreeUnit
        => IsMetric ? "°C" : "°F";

    public string EmojiForWeather(WeatherType weather, bool isSunUp = true)
    {
        if (isSunUp)
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
                    return "☀️";
            }
        }
        else
        {
            switch (weather)
            {
                //no emoji with cloud over moon, so just show a cloud
                case WeatherType.ScatteredClouds:
                case WeatherType.PartlyCloudy:
                case WeatherType.Cloudy:
                case WeatherType.Overcast:
                    return "☁️";
                case WeatherType.Rain:
                    return "☔️";
                case WeatherType.Snow:
                    return "❄️";
                case WeatherType.Thunderstorm:
                    return "⛈";
                default:
                    //clear, so show moon
                    //todo: add phases of moon?
                    return "🌕";
            }
        }
    }

    public string FormatDescription(Weather weather)
        => culture.TextInfo.ToTitleCase(weather.Description);
}
