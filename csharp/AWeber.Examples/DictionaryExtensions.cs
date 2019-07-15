using System;
using System.Collections.Generic;
using System.Linq;

namespace AWeber.Examples
{
    public static class DictionaryExtensions
    {
        public static string ToUrlParams(this IDictionary<string, string> paramsDictionary)
        {
            return string.Join("&", paramsDictionary.Select(kvp => string.Format("{0}={1}", kvp.Key, Uri.EscapeUriString(kvp.Value))));
        }
    }
}