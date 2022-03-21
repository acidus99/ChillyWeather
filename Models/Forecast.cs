using System;

namespace WeatherCat.Models
{
    /// <summary>
    /// A condition at a specific point in time
    /// </summary>
    public class CurrentCondition
    {
        public DateTime Time { get; set; }
        public float Temp { get; set; }
        public Weather Weather { get; set; }
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }
    }

    public class Weather
    {
        public string Description { get; set; }
        public WeatherType Type { get; set; }
    }

    public class HourlyCondition : CurrentCondition
    {
        /// <summary>
        /// 0-1.0: % chance of Precipitation
        /// </summary>
        public float ChanceOfPrecipitation { get; set; }
    }

    public class DailyCondition
    {
        public DateTime Time { get; set; }
        public Weather Weather { get; set; }

        public float HighTemp { get; set; }
        public float LowTemp { get; set; }

        /// <summary>
        /// 0-1.0: % chance of Precipitation
        /// </summary>
        public float ChanceOfPrecipitation { get; set; }
    }

    public class Forecast
    {
        public GeoLocale Location;

        public CurrentCondition Current;

        public HourlyCondition [] Hourly;

        public DailyCondition[] Daily;
    }

    public enum WeatherType
    {
        Clear,

        ScatteredClouds, //0-25
        PartlyCloudy, //25-50
        Cloudy, //50-85
        Overcast, //85-100

        Rain,
        Thunderstorm,
        Snow,
    }

}
