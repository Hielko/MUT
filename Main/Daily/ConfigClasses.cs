using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUT.Daily
{
    public class DailySettings
    {
        // Setting it to 0 passes all msgs!
        public int ChanceMultiplier { get; set; } = 1;
        public DateTime resetTime;
        public Boolean Enabled { get; set; } = true;
        public String ResetTime
        {
            set => resetTime = DateTime.ParseExact(value, "HH:mm", CultureInfo.InvariantCulture);
        }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class MsgTimeSpan
    {
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public double TotalMinutes { get; private set; }

        public String Time
        {
            set
            {
                String[] parts = value.Split('-');
                FromTime = DateTime.ParseExact(parts[0], "HH:mm", CultureInfo.InvariantCulture);
                ToTime = DateTime.ParseExact(parts[1], "HH:mm", CultureInfo.InvariantCulture);
                if (ToTime < FromTime)
                {
                    TotalMinutes = ToTime.AddDays(1).Subtract(FromTime).TotalMinutes;
                }
                else
                {
                    TotalMinutes = ToTime.Subtract(FromTime).TotalMinutes;
                }
            }
        }

        public override string ToString()
        {
            return String.Format("{0} to {1}", FromTime.ToString("HH:mm"), ToTime.ToString("HH:mm"));
        }
    }

    public class Message
    {
        public int Chance { get; set; }
        public String Text { get; set; }
        public override string ToString()
        {
            return String.Format("Chance={0}, Text={1}", Chance, Text);
        }
    }

    public class MessageGroup
    {
        public MsgTimeSpan BotTime { get; set; }
        public String Time
        {
            set
            {
                BotTime = new MsgTimeSpan { Time = value };
            }
        }

        public List<DayOfWeek> DaysList = new List<DayOfWeek>();
        public String Days
        {
            set => DaysList = Utils.DateUtils.ParseDaysOfWeek(value);
        }

        public int Chance { get; set; }
        public Message[] Msgs { get; set; }
        public override string ToString()
        {
            var result = String.Format("Time={0}, Chance={1}", BotTime, Chance) + "\n";
            foreach (var s in Msgs) result += s + ",\n";
            return result + "\n";
        }
    }

    public class DailyConfig
    {
        public DailySettings Settings { get; set; } = new DailySettings();
        public List<MessageGroup> Messages { get; set; } = new List<MessageGroup>();
        public override string ToString()
        {
            return String.Format("messages.count = {0}, settings = {1}", Messages.Count, Settings);
        }
    }

}
