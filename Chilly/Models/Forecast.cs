﻿using System;
using System.Linq;

namespace Chilly.Models
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
        public required string Description { get; set; }
        public required WeatherType Type { get; set; }
    }

    public class HourlyCondition : CurrentCondition
    {
        /// <summary>
        /// 0-1.0: % chance of Precipitation
        /// </summary>
        public float ChanceOfPrecipitation { get; set; }
    }

    public class RemainingToday
    {
        public int RemainingHours { get; set; }
        public float HighTemp { get; set; }
        public float LowTemp { get; set; }

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

        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }

        /// <summary>
        /// 0-1.0: % chance of Precipitation
        /// </summary>
        public float ChanceOfPrecipitation { get; set; }
    }

    public class Forecast
    {
        public GeoLocale Location { get; set; }

        public CurrentCondition Current { get; set; }

        public HourlyCondition [] Hourly { get; set; }

        public DailyCondition[] Daily { get; set; }

        public bool IsMetric { get; set; }

        public RemainingToday GetRemainingToday()
        {
            var remainingHours = Hourly.Where(x => (x.Time >= Current.Time && x.Time.Date == Current.Time.Date));
            if (remainingHours.Count() > 0)
            {
                return new RemainingToday
                {
                    RemainingHours = remainingHours.Count(),
                    LowTemp = remainingHours.Min(x => x.Temp),
                    HighTemp = remainingHours.Max(x => x.Temp),
                    ChanceOfPrecipitation = remainingHours.Max(x => x.ChanceOfPrecipitation)
                };
            } else
            {
                return null;
            }
        }

        public DailyCondition GetDailyCondition(DateTime time)
        {
            foreach (DailyCondition daily in Daily)
            {
                if (daily.Time.Date == time.Date)
                {
                    return daily;
                }
            }
            return null;
        }

        /// <summary>
        /// Determines if a datetime occuring when the sun is up. If we don't
        /// have sunrise data for it, return true
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsSunUp(DateTime time)
        {
            foreach(DailyCondition daily in Daily)
            {
                if(daily.Time.Date == time.Date)
                {
                    if(time < daily.Sunrise || time > daily.Sunset)
                    {
                        return false;
                    }
                    return true;
                }
            }
            //if we get here, something is wrong with the forecast, so default to true
            return true;
        }

        public bool IsSunCurrentlyUp
            => IsSunUp(Current.Time);
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
