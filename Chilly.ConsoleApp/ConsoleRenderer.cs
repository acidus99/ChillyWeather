namespace Chilly.ConsoleApp;

using Chilly.Models;

/// <summary>
/// Implements forecast rendering to the console.
/// </summary>
public class ConsoleRenderer : AbstractForcastRenderer
{
    public ConsoleRenderer(TextWriter fout)
        : base(fout) { }

    public override void Render(Forecast forecast)
    {
        _fout.WriteLine($"Weather for {forecast.Location.Name} @ {_formatter.FormatDayTime(forecast.Current.Time)}");
        _fout.WriteLine($"{_formatter.EmojiForWeather(forecast.Current.Weather.Type, forecast.IsSunCurrentlyUp)} {_formatter.FormatTemp(forecast.Current.Temp)}");
        _fout.WriteLine($"{_formatter.FormatDescription(forecast.Current.Weather)}");

        _fout.WriteLine();
        foreach(DailyCondition daily in forecast.Daily)
        {
            if(daily.Time < forecast.Current.Time)
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
}
