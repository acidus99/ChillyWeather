using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

using Chilly.Clients;
using Chilly.Models;
using Gemini.Cgi;

namespace ChillyCgi
{
    class Program
    {
        const string chillyPath = "/cgi-bin/chilly.cgi";

        static void Main(string[] args)
        {
            if(!CgiWrapper.IsRunningAsCgi)
            {
                TestLocal();
                return;
            }

            CgiRouter router = new CgiRouter();

            router.OnRequest("/search", Search);
            router.OnRequest("/view", WeatherForLocale);
            router.OnRequest("/", WeatherForCurrentLocation);
            router.SetStaticRoot("static/");
            router.ProcessRequest();
        }

        static void TestLocal()
        {
            using (var cgi = new CgiWrapper())
            {
                WeatherForLocale(cgi);
            }
            GeoLocale locale = new GeoLocale
            {
                Latitude = 44.0077859,
                Longitude = -97.6962364,
                Country = "US"
            };

            var client = ClientForge.ConfigureWeatherClient();
            client.GetForecast(locale);

        }

        static void WeatherForCurrentLocation(CgiWrapper cgi)
        {
            ILocaleClient localeClient = new IpApiClient();
            string ip = GetRemoteIP(cgi);

            GeoLocale locale = localeClient.GetIPLocale(ip);
            RenderWeather(cgi, locale);
        }

        static void Search(CgiWrapper cgi)
        {
            if(!cgi.HasQuery)
            {
                cgi.Input("Enter Location to search for weather forecast (e.g. 'city, country' or 'city, state, country'");
                return;
            }

            IWeatherClient client = ClientForge.ConfigureWeatherClient();
            var locales = client.LookupLocale(cgi.Query.Trim());

            cgi.Success();
            cgi.Writer.WriteLine("## Discovered locations");
            if (locales.Count > 0)
            {
                foreach (var locale in locales)
                {
                    cgi.Writer.WriteLine($"=> {chillyPath}/view/{HttpUtility.UrlEncode(GeoToString(locale))}/ {locale.Name}, {locale.State}, {locale.Country}");
                }
            } else
            {
                cgi.Writer.WriteLine("No results found. Try being more specific, such as 'city, country' or 'city, state, country'");
            }
            cgi.Writer.WriteLine();
            cgi.Writer.WriteLine($"=> {chillyPath}/search Search Again");
            cgi.Writer.WriteLine($"=> {chillyPath} Use Current Location");
            Footer(cgi);
        }

        static string GeoToString(GeoLocale locale)
        {
            NameValueCollection query = new NameValueCollection();
            query["lon"] = locale.Longitude.ToString();
            query["lat"] = locale.Latitude.ToString();
            query["name"] = locale.Name;
            query["state"] = locale.State;
            query["country"] = locale.Country;
            return string.Join("&", query.AllKeys.Select(key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(query[key]))));
        }


        static GeoLocale GetGeo(CgiWrapper cgi)
        {
            try
            {
                var locale = cgi.PathInfo.Substring(6, cgi.PathInfo.Length - 7);
                var query = HttpUtility.ParseQueryString(locale);
                return new GeoLocale
                {
                    Longitude = Convert.ToSingle(query["lon"]),
                    Latitude = Convert.ToSingle(query["lat"]),
                    Name = query["name"],
                    State = query["state"],
                    Country = query["country"]
                };
            }
            catch (Exception)
            {
                return null;
            }
        }

        static void WeatherForLocale(CgiWrapper cgi)
        {

            var locale = GetGeo(cgi);
            if(locale == null)
            {
                cgi.Success();
                cgi.Writer.WriteLine("could not find location");
                return;
            }
            RenderWeather(cgi, locale);
        }

        static void RenderWeather(CgiWrapper cgi, GeoLocale locale)
        {
            IWeatherClient client = ClientForge.ConfigureWeatherClient();

            bool isMetric = (locale.Country != "US");

            if (cgi.Query == "c")
            {
                isMetric = true;
            }
            else if (cgi.Query == "f")
            {
                isMetric = false;
            }

            var forecast = client.GetForecast(locale, isMetric);
            cgi.Success();

            GeminiRenderer renderer = new GeminiRenderer(cgi.Writer);
            renderer.Render(forecast);
            Footer(cgi);
        }

        static string GetRemoteIP(CgiWrapper cgi)
            => cgi.RemoteAddress == "127.0.01" ? "" : cgi.RemoteAddress;

        static void Footer(CgiWrapper cgi)
        {
            cgi.Writer.WriteLine("--");
            cgi.Writer.WriteLine($"=> {chillyPath}/about.gmi About ⛄️ Chilly Weather");
            cgi.Writer.WriteLine("=> mailto:acidus@gemi.dev Made with ❄️ and ❤️ by Acidus");
        }
    }
}
