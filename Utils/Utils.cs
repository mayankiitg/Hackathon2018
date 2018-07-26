using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace SimpleEchoBot.Utils
{
    public static class Utils
    {
        public static Dictionary<string, string> convertResponseToMap(Object obj)
        {
            string response = obj.ToString();
            response = Regex.Replace(response, @"(\r\n)|\n|\r","");
            Dictionary<string, string> responseMap =  JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
            return responseMap;
        }

        public static int ConvertToInt(string s)
        {
            int output = 0;
            if (int.TryParse(s, out output))
            {
                return output;
            }

            return -1;
        }
    }
}