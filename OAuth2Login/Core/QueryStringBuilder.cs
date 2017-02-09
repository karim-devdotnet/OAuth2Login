using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OAuth2Login.Core
{
    public class QueryStringBuilder
    {
        public static string BuildCompex(string[] dontEncode, params object[] keyValueEntries)
        {
            if (keyValueEntries.Length % 2 != 0)
            {
                throw new Exception(
                    "KeyAndValue collection needs to be dividable by two... key, value, key, value... get it?");
            }

            var sb = new StringBuilder();
            for (int i = 0; i < keyValueEntries.Length; i += 2)
            {
                var key = keyValueEntries[i];
                var val = keyValueEntries[i + 1];

                var valEncoded = HttpUtility.UrlEncode(val.ToString());
                if (dontEncode != null && dontEncode.Contains(key))
                    valEncoded = val.ToString();

                sb.AppendFormat("{0}={1}&", key, valEncoded);
            }

            return sb.ToString().TrimEnd('&');
        }

        public static string Build(params object[] keyValueEntries)
        {
            return BuildCompex(null, keyValueEntries);
        }

        internal static string BuildFromDictionary(Dictionary<string, object> paramsDict, bool orderByAlphabet)
        {
            var keysList = paramsDict.Keys.ToList();
            keysList.Sort();

            var queryList = new List<object>();
            foreach (var key in keysList)
            {
                queryList.Add(key);
                queryList.Add(paramsDict[key]);
            }

            return BuildCompex(null, queryList.ToArray());
        }
    }
}
