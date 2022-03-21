using System.Collections.Generic;
using System.Net;
using System;
using System.Linq;

using WeatherCat.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeatherCat.Clients
{
    public class OpenWeatherClient : IWeatherClient
    {

        string ApiKey;

        WebClient client = new WebClient();

        public OpenWeatherClient(string apiKey)
        {
            ApiKey = apiKey;
        }

        public List<GeoLocale> LookupLocale(string query)
        {
            var url = $"http://api.openweathermap.org/geo/1.0/direct?q={u(query)}&limit=5&appid={ApiKey}";

            var json = client.DownloadString(url);
            var resp = JsonConvert.DeserializeObject<GeoLocale[]>(json);

            List<GeoLocale> ret = new List<GeoLocale>();
            ret.AddRange(resp);
            return ret;
        }

        public Forecast GetForecast(GeoLocale locale)
        {
            var json = WeatherOneCallLookup(locale);
            JObject root = JObject.Parse(json);

            int timeOffset = ((int?)root["timezone_offset"]) ?? 0;

            return new Forecast
            {
                Location = locale,
                Current = ParseCurrent(root["current"] as JObject, timeOffset),
                Hourly = ParseHourly(root["hourly"] as JArray, timeOffset),
                Daily = ParseDaily(root["daily"] as JArray, timeOffset),
            };
        }

        private DateTime AdjustDateTime(JToken time, int timeOffset)
        {
            int ctime = (int)time;
            int localCtime = ctime + timeOffset;

            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(localCtime);
            return dateTime;
        }

        private CurrentCondition ParseCurrent(JObject current, int timeOffset)
        {
            return new CurrentCondition
            {
                Time = AdjustDateTime(current["dt"], timeOffset),
                Sunrise = AdjustDateTime(current["sunrise"], timeOffset),
                Sunset = AdjustDateTime(current["sunset"], timeOffset),
                Temp = ((float)current["temp"]),
                Weather = ParseWeather(current["weather"] as JArray)
            };
        }

        private HourlyCondition [] ParseHourly(JArray hourly, int timeOffset)
        {
            var hourlyConditions = new List<HourlyCondition>();
            foreach(JObject hour in hourly)
            {
                hourlyConditions.Add(new HourlyCondition
                {
                    Time = AdjustDateTime(hour["dt"], timeOffset),
                    Temp = ((float)hour["temp"]),
                    Weather = ParseWeather(hour["weather"] as JArray),
                    ChanceOfPrecipitation = ((float)hour["pop"])
                });
            }
            return hourlyConditions.ToArray();
        }

        private DailyCondition[] ParseDaily(JArray daily, int timeOffset)
        {
            var dailyConditions = new List<DailyCondition>();
            foreach (JObject day in daily)
            {
                dailyConditions.Add(new DailyCondition
                {
                    Time = AdjustDateTime(day["dt"], timeOffset),
                    LowTemp =((float)day["temp"]["min"]),
                    HighTemp = ((float)day["temp"]["max"]),
                    Weather = ParseWeather(day["weather"] as JArray),
                    ChanceOfPrecipitation = ((float)day["pop"]),
                });
            }
            return dailyConditions.ToArray();
        }

        private Weather ParseWeather(JArray weatherArray)
        {
            var weather = (weatherArray[0] as JObject);
            return new Weather
            {
                Description = weather["description"].ToString(),
                Type = ParseWeatherID(((int)weather["id"]))
            };
        }

        private WeatherType ParseWeatherID(int weatherID)
        {
            if(weatherID >= 200 && weatherID < 232)
            {
                return WeatherType.Thunderstorm;
            } else if (weatherID >= 300 && weatherID < 321)
            {
                return WeatherType.Rain;
            }
            else if (weatherID >= 500 && weatherID < 531)
            {
                return WeatherType.Rain;
            }
            else if (weatherID >= 600 && weatherID < 622)
            {
                return WeatherType.Snow;
            }
            else if (weatherID == 801)
            {
                return WeatherType.ScatteredClouds;
            }
            else if (weatherID == 802)
            {
                return WeatherType.PartlyCloudy;
            }
            else if (weatherID == 803)
            {
                return WeatherType.Cloudy;
            }
            else if (weatherID == 804)
            {
                return WeatherType.Overcast;
            }
            return WeatherType.Clear;
        }

        private string u(string s)
            => WebUtility.UrlEncode(s);

        private string WeatherOneCallLookup(GeoLocale geoLocale)
        => WeatherOneCallLookup(geoLocale.Longitude, geoLocale.Latitude);

        private string WeatherOneCallLookup(double lon, double lat)
        {
            var url = $"https://api.openweathermap.org/data/2.5/onecall?lat={lat}&lon={lon}&exclude=minutely&units=imperial&appid={ApiKey}";
            var json = client.DownloadString(url);
            return json;
        }
    }
}
