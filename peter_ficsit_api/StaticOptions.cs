using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace peter_ficsit_api
{
    public static class StaticOptions
    {
        public static string APIKEY { get; set; }
        public static string APIURL { get; set; }

        public static string SiteURL()
        {
            return APIURL.Replace("/v2/query", "");
        }

        public static ILogger Logger { get; set; }
    }
}
