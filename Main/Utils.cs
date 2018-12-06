using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Utils
{

    public class ResetTimer
    {
        private System.Timers.Timer tmrReload;
        public event EventHandler Elapsed;

        public ResetTimer(DateTime time, Func<int> callBack)
        {
            if (time == null)
                return;

            var resetTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, time.Hour, time.Minute, 0);
            if (resetTime < DateTime.Now)
                resetTime = resetTime.AddDays(1);
            TimeSpan ts = resetTime - DateTime.Now;
            tmrReload = new System.Timers.Timer
            {
                Interval = ts.TotalMinutes * (60 * 1000),
                AutoReset = false,
                Enabled = true
            };
            tmrReload.Elapsed += delegate (Object o, ElapsedEventArgs e)
            {
                callBack();
            };
            tmrReload.Start();
        }
    }

    public static class Common
    {
        public static int Random(int max)
        {
            return new Random(Guid.NewGuid().GetHashCode()).Next(max);
        }
    }

    public static class StringUtils
    {
        public static Boolean IsAllUppercase(string str)
        {
            foreach (var c in str)
            {
                if (Char.IsLower(c)) return false;
            }
            return true;
        }


        public static String StripUTF(string str)
        {
            string result = "";
            char[] p = str.ToCharArray();
            for (var i = 1; i < p.Length; i += 2)
            {
                result += p[i];
            }
            return result;
        }

        public static String RandomOneOf(params string[] some)
        {
            return some[new Random(Guid.NewGuid().GetHashCode()).Next(some.Length)];
        }


        // "{Hi, Hello, Goodday} You!"
        public static String ReplaceRandomTokens(String s)
        {
            if (String.IsNullOrEmpty(s))
                return s;

            String result = s;
            int p1 = s.LastIndexOf('{');
            if (p1 != -1)
            {
                int p2 = s.Substring(p1).IndexOf('}');
                if (p2 != -1)
                {
                    string tokens = s.Substring(p1 + 1, p2 - 1);
                    var tokensArray = tokens.Split(',').ToList();
                    if (tokensArray.Count == 1) tokensArray.Add("");
                    result = s.Substring(0, p1) + RandomOneOf(tokensArray.ToArray()) + s.Substring(p1 + p2 + 1);
                    result = result.Replace("  ", " ");
                    if (result.IndexOf('{') != -1)
                        return ReplaceRandomTokens(result);
                }
            }
            return result;
        }





    }


    public static class DateUtils
    {
        public static List<DayOfWeek> ParseDaysOfWeek(String strDays)
        {
            var result = new List<DayOfWeek>();
            foreach (string strDay in strDays.Split(','))
            {
                if (Enum.TryParse<DayOfWeek>(strDay, true, out DayOfWeek day))
                {
                    result.Add(day);
                }
            }
            return result;
        }
    }

}
