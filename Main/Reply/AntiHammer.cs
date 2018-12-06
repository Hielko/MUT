using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUT
{
    public static class AntiHammer
    {
        private static DateTime LastDateTime = DateTime.MinValue;

        public static bool HammerDetected(int antiHammerSeconds = 0)
        {
            var result = false;
            if (antiHammerSeconds > 0)
            {
                var Span = DateTime.Now.Subtract(LastDateTime);
                if (Span.TotalSeconds < antiHammerSeconds)
                {
                    result = true;
                }
            }
            LastDateTime = DateTime.Now;
            return result;
        }
    }
}
