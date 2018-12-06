using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUT.Common
{
    public class CommonConfig
    {
        public int AllDisabled { get; set; } = 0;
        public DateTime resetTime;
        public String ResetTime
        {
            set => resetTime = DateTime.ParseExact(value, "HH:mm", CultureInfo.InvariantCulture);
        }
        public Dictionary<String, String> variables = new Dictionary<string, string>();
    }
}
