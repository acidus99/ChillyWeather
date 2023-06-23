using System;
using System.Linq;
using System.Globalization;
using System.IO;

using Chilly.Models;
using Chilly;


namespace ChillyCgi
{
    public class GeminiRenderer
    {
        TextWriter Fout;
        Formatter formatter;

        public GeminiRenderer(TextWriter fout)
        {
            Fout = fout;
            formatter = new Formatter();
        }

        public void Render(Forecast forecast)
        {
            formatter.IsMetric = forecast.IsMetric;

            Fout.WriteLine($"# ⛄️ Chilly Weather: {forecast.Location.Name}");
            Fout.WriteLine($"Weather for {forecast.Location.Name} @ {formatter.FormatDayTime(forecast.Current.Time)}");

            Fout.WriteLine("=> /cgi-bin/chilly.cgi/search Wrong Location? Search");

            if (formatter.IsMetric)
            {
                Fout.WriteLine("=> ?f Use Fahrenheit");
            }
            else
            {
                Fout.WriteLine("=> ?c Use Celsius");
            }

            Fout.WriteLine($"Now: {formatter.EmojiForWeather(forecast.Current.Weather.Type, forecast.IsSunCurrentlyUp)} {formatter.FormatTemp(forecast.Current.Temp)} {formatter.FormatDescription(forecast.Current.Weather)}");
            var remainingToday = forecast.GetRemainingToday();
            if(remainingToday != null && remainingToday.RemainingHours > 4)
            {
                Fout.Write($"Rest of Today: {formatter.FormatTemp(remainingToday.LowTemp)} to {formatter.FormatTemp(remainingToday.HighTemp)}");
                if(remainingToday.ChanceOfPrecipitation > 0)
                {
                    Fout.Write($"💧 {formatter.FormatChance(remainingToday.ChanceOfPrecipitation)}");
                }
                Fout.WriteLine();
            }
            Fout.WriteLine();
            Fout.WriteLine("## Next 24 hours");

            DateTime last = DateTime.Now;
            //skip every other hour
            int i = 0;
            foreach (HourlyCondition hour in forecast.Hourly.Skip(2).Take(24))
            {
                i++;
                if(i % 2 ==0)
                {
                    continue;
                }
                //get Daily condition for hour
                var daily = forecast.GetDailyCondition(hour.Time);
                RenderSunChange(hour, daily, last);
                Fout.Write($"* {formatter.FormatHour(hour.Time)}: {formatter.EmojiForWeather(hour.Weather.Type, forecast.IsSunUp(hour.Time))} {formatter.FormatTemp(hour.Temp)} ");
                if(hour.ChanceOfPrecipitation != 0)
                {
                    Fout.Write($"💧 {formatter.FormatChance(hour.ChanceOfPrecipitation)} - ");
                }
                Fout.WriteLine(formatter.FormatDescription(hour.Weather));
                last = hour.Time;
            }

            Fout.WriteLine();

            Fout.WriteLine("## Next 7 Days");
            foreach(DailyCondition daily in forecast.Daily)
            {
                if (daily.Time.Date <= forecast.Current.Time.Date)
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

        private void RenderSunChange(HourlyCondition hour, DailyCondition daily, DateTime lastUpdate)
        {
            //did sunset occur between last and current
            if (OccurredBetween(lastUpdate, hour.Time, daily.Sunrise))
            {
                Fout.WriteLine($"* {formatter.FormatTime(daily.Sunrise)}: 🌅 Sunrise");
            } else if (OccurredBetween(lastUpdate, hour.Time, daily.Sunset))
            {
                Fout.WriteLine($"* {formatter.FormatTime(daily.Sunset)}: 🌅 Sunset");
            }
        }

        private bool OccurredBetween(DateTime lowEnd, DateTime highEnd, DateTime check)
            => lowEnd <= check && check <= highEnd;
    }
}
