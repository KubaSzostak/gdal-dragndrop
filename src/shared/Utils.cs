using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace System
{

    public class Utils
    {
        public static char[] SplitChars = " ,;\t".ToCharArray();

        public static string[] Split(string s)
        {
            return s.Split(SplitChars, StringSplitOptions.RemoveEmptyEntries);
        }

        public static double ToDouble(string s)
        {
            return double.Parse(s, CultureInfo.InvariantCulture);
        }
    }
}
