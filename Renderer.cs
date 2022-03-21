using System;
using System.Globalization;
using System.IO;

using Chilly.Models;

namespace Chilly
{
    public class Renderer
    {
        TextWriter Fout;
        Formatter formatter;

        public Renderer(TextWriter fout)
        {
            Fout = fout;
            formatter = new Formatter();
        }

        public void Render(Forecast forecast)
        {
            Fout.WriteLine($"Weather for {forecast.Location.Name} @ {formatter.FormatTime(forecast.Current.Time)}");
            Fout.WriteLine($"{formatter.EmojiForCurrentWeather(forecast.Current)} {formatter.FormatTemp(forecast.Current.Temp)}");
            Fout.WriteLine($"{formatter.FormatDescription(forecast.Current.Weather)}");

            Fout.WriteLine();
            foreach(DailyCondition daily in forecast.Daily)
            {
                if(daily.Time < forecast.Current.Time)
                {
                    continue;
                }
                Fout.WriteLine($"{formatter.FormatDay(forecast.Current.Time, daily.Time)}: {formatter.EmojiForWeather(daily.Weather.Type)} {formatter.FormatTemp(daily.LowTemp)} to {formatter.FormatTemp(daily.HighTemp)}");
                if (daily.ChanceOfPrecipitation == 0)
                {
                    Fout.WriteLine($"\t{formatter.FormatDescription(daily.Weather)}");
                }
                else
                {
                    Fout.WriteLine($"\t💧 {formatter.FormatChance(daily.ChanceOfPrecipitation)} - {formatter.FormatDescription(daily.Weather)}");
                }
                Fout.WriteLine();
            }
        }
    }
}
