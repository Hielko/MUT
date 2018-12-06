using MUT.Daily;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace MUT.Reply
{
    public class ReplySettings
    {
        public int TypoChance { get; set; } = 0;
        public int NoReplyChance { get; set; } = 0;
        public int ReloadMinutes { get; set; } = 0;
        public int AntiHammerSeconds { get; set; }
        public int ALLCAPSRepeatChance { get; set; } = 0;
        public int DelaySeconds { get; set; } = 60;
        public int DelayDefaultReplySeconds { get; set; } = 300;
        public Boolean Enabled { get; set; } = true;
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class BlockPeriod
    {
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            RandomMinutes = Utils.Common.Random(MaxRandomMinutes);
            if (Utils.Common.Random(1) == 0)
            {
                RandomMinutes *= -1;
            }
        }

        public List<DayOfWeek> BlockDays = new List<DayOfWeek>();
        public String Days
        {
            set => BlockDays = Utils.DateUtils.ParseDaysOfWeek(value);
        }
        private MsgTimeSpan BlockTimeSpan { get; set; }
        public String Time
        {
            set => BlockTimeSpan = new MsgTimeSpan { Time = value };
        }

        public int MaxRandomMinutes { get; set; } = 0;
        public int RandomMinutes = 0;

        public Boolean IsMatch(DateTime date)
        {
            if ((BlockDays.Count > 0) && (BlockTimeSpan == null))
            {
                return BlockDays.Contains(date.DayOfWeek);
            }

            Boolean result = false;

            if (BlockTimeSpan != null)
            {
                var testdate = new DateTime(0).AddHours(date.Hour).AddMinutes(date.Minute);
                var fromDate = new DateTime(0).AddHours(BlockTimeSpan.FromTime.Hour).AddMinutes(BlockTimeSpan.FromTime.Minute + RandomMinutes);
                var toDate = fromDate.AddMinutes(BlockTimeSpan.TotalMinutes + RandomMinutes);

                if ((BlockTimeSpan.ToTime < BlockTimeSpan.FromTime))
                {
                    if (testdate > fromDate)
                    {
                        result = true;
                    }
                    if (testdate.AddDays(1) < toDate)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = (testdate > fromDate) && (testdate < toDate);
                }

                if (BlockDays.Count > 0)
                {
                    result = result && BlockDays.Contains(date.DayOfWeek);
                }
            }
            return result;
        }
    }

    public class DefaultReply
    {
        public int Chance { get; set; }
        public String ReplyText { get; set; }
    }

    public class RuleGroup
    {
        public String[] Contains { get; set; }
        public String[] Startswith { get; set; }
        public String[] Endswidth { get; set; }
        public String ReplyText { get; set; }

        public Boolean IsMatch(String text)
        {
            var upperText = text.ToUpper();
            bool result = false;
            if (Contains != null)
            {
                result = Contains.Count(s => upperText.Contains(s.ToUpper())) > 0;
            }
            else
            {
                if (!result && Startswith != null)
                {
                    result = Startswith.Count(s => upperText.StartsWith(s.ToUpper())) > 0;
                    if (!result && Endswidth != null)
                    {
                        result = Endswidth.Count(s => upperText.EndsWith(s.ToUpper())) > 0;
                    }
                }
            }
            return result;
        }
    }

    public class AllCapsReply : DefaultReply
    {
        public int MinLength { get; set; } = 4;
    }

    public class Specials
    {
        public DefaultReply repeatreply { get; set; }
        public AllCapsReply allcapsreply { get; set; }
    }

    public class ReplyConfig
    {
        public ReplySettings Settings { get; set; } = new ReplySettings();
        public List<BlockPeriod> BlockPeriods { get; set; } = new List<BlockPeriod>();
        public List<RuleGroup> replies { get; set; } = new List<RuleGroup>();
        public List<DefaultReply> defaults { get; set; } = new List<DefaultReply>();
        public Specials specials { get; set; } = new Specials();
        public override string ToString()
        {
            return String.Format("ruleGroups.count = {0}, defaultReplies.count = {1}, settings = {2}", replies.Count, defaults.Count, Settings);
        }
    }

}
