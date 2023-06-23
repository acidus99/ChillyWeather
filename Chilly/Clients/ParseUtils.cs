namespace Chilly.Clients;

using System;
using Newtonsoft.Json.Linq;

internal static class ParseUtils
{
    internal static string Cleanse(JToken? token)
       => token?.ToString() ?? "";

    internal static double GetCoordinate(JToken? token)
        => (token != null) ? Convert.ToDouble(token.ToString()) : 0;
}

