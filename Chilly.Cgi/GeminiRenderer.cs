namespace Chilly.Cgi;

using System;
using System.IO;
using System.Linq;

using Chilly;
using Chilly.Models;

public class GeminiRenderer : AbstractForcastRenderer
{
    public GeminiRenderer(TextWriter _fout)
        : base(_fout) { }

    public override void Render(Forecast forecast)
    {
        _formatter.IsMetric = forecast.IsMetric;

        _fout.WriteLine($"# ⛄️ Chilly Weather: {forecast.Location.Name}");
        _fout.WriteLine($"Weather for {forecast.Location.Name} @ {_formatter.FormatDayTime(forecast.Current.Time)}");

        _fout.WriteLine("=> /cgi-bin/chilly.cgi/search Wrong Location? Search");

        if (_formatter.IsMetric)
        {
            _fout.WriteLine("=> ?f Use Fahrenheit");
        }
        else
        {
            _fout.WriteLine("=> ?c Use Celsius");
        }

        _fout.WriteLine($"Now: {_formatter.EmojiForWeather(forecast.Current.Weather.Type, forecast.IsSunCurrentlyUp)} {_formatter.FormatTemp(forecast.Current.Temp)} {_formatter.FormatDescription(forecast.Current.Weather)}");
        var remainingToday = forecast.GetRemainingToday();
        if(remainingToday != null && remainingToday.RemainingHours > 4)
        {
            _fout.Write($"Rest of Today: {_formatter.FormatTemp(remainingToday.LowTemp)} to {_formatter.FormatTemp(remainingToday.HighTemp)}");
            if(remainingToday.ChanceOfPrecipitation > 0)
            {
                _fout.Write($"💧 {_formatter.FormatChance(remainingToday.ChanceOfPrecipitation)}");
            }
            _fout.WriteLine();
        }
        _fout.WriteLine();
        _fout.WriteLine("## Next 24 hours");

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
            _fout.Write($"* {_formatter.FormatHour(hour.Time)}: {_formatter.EmojiForWeather(hour.Weather.Type, forecast.IsSunUp(hour.Time))} {_formatter.FormatTemp(hour.Temp)} ");
            if(hour.ChanceOfPrecipitation != 0)
            {
                _fout.Write($"💧 {_formatter.FormatChance(hour.ChanceOfPrecipitation)} - ");
            }
            _fout.WriteLine(_formatter.FormatDescription(hour.Weather));
            last = hour.Time;
        }

        _fout.WriteLine();

        _fout.WriteLine("## Next 7 Days");
        foreach(DailyCondition daily in forecast.Daily)
        {
            if (daily.Time.Date <= forecast.Current.Time.Date)
            {
                continue;
            }
            _fout.WriteLine($"{_formatter.FormatDay(forecast.Current.Time, daily.Time)}: {_formatter.EmojiForWeather(daily.Weather.Type)} {_formatter.FormatTemp(daily.LowTemp)} to {_formatter.FormatTemp(daily.HighTemp)}");
            if (daily.ChanceOfPrecipitation == 0)
            {
                _fout.WriteLine($"\t{_formatter.FormatDescription(daily.Weather)}");
            }
            else
            {
                _fout.WriteLine($"\t💧 {_formatter.FormatChance(daily.ChanceOfPrecipitation)} - {_formatter.FormatDescription(daily.Weather)}");
            }
            _fout.WriteLine();
        }
    }

    private void RenderSunChange(HourlyCondition hour, DailyCondition daily, DateTime lastUpdate)
    {
        //did sunset occur between last and current
        if (OccurredBetween(lastUpdate, hour.Time, daily.Sunrise))
        {
            _fout.WriteLine($"* {_formatter.FormatTime(daily.Sunrise)}: 🌅 Sunrise");
        } else if (OccurredBetween(lastUpdate, hour.Time, daily.Sunset))
        {
            _fout.WriteLine($"* {_formatter.FormatTime(daily.Sunset)}: 🌅 Sunset");
        }
    }

    private bool OccurredBetween(DateTime lowEnd, DateTime highEnd, DateTime check)
        => lowEnd <= check && check <= highEnd;
}
