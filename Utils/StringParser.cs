using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Izzy
{
    public static class StringUtils
    {
        public static char GetLastCharacter(string str)
        {
            string trimmedString = str.Trim();
            return trimmedString[trimmedString.Length - 1];
        }
        public static string RemoveWhitespace(string str)
        {
            return Regex.Replace(str, @"\s+", "");
        }
    }

}