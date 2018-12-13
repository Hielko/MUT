using System;
using System.Collections.Generic;
using System.Linq;
using MUT.Common;
using Utils;

namespace MUT.Daily
{

    public class DailyModule : AModule<DailyConfig>
    {
        public const string Filename = "daily.json";
        public ResetTimer resetTimer;
        public event EventHandler Reset;
      
        private void InitTimer()
        {
            if (Config.Settings.resetTime == null)
                return;

            resetTimer = new ResetTimer(Config.Settings.resetTime, delegate ()
            {
                LoadConfig();
                Reset?.Invoke(this, null);
                return 0;
            });
        }


        public DailyModule(Location location) : base(location)
        {
            Name = "Daily";
            InitTimer();
        }

        public List<OutgoingMsg> Generate(CommonModule commonModule)
        {
            var outgoingMsgList = new List<OutgoingMsg>();

            if (!Config.Settings.Enabled)
            {
                return outgoingMsgList;
            }

            List<MessageGroup> messageGroups = Config.Messages.FindAll(mg => mg.DaysList.Count == 0 || mg.DaysList.Exists(dayOfWeek => dayOfWeek.Equals(DateTime.Now.DayOfWeek)));
            messageGroups = messageGroups.FindAll(mg => Utils.Common.Random(mg.Chance * Config.Settings.ChanceMultiplier) == 0);

            messageGroups.ForEach(delegate (MessageGroup messageGroup)
            {
                DateTime executeTime = messageGroup.BotTime.FromTime.AddMinutes(Utils.Common.Random((int)messageGroup.BotTime.TotalMinutes));
                var msgs = messageGroup.Msgs.ToList().FindAll(mg => Utils.Common.Random(mg.Chance) == 0);

                msgs.ForEach(delegate (Message message)
                {
                    var text = Parse.DoParse(message.Text, commonModule.Config.variables);
                    foreach (var s in Utils.StringUtils.ReplaceRandomTokens(text).Split('\n'))
                    {
                        executeTime = executeTime.AddSeconds(Utils.Common.Random(20) + 5);
                        var outgoingMsg = new OutgoingMsg
                        {
                            MsgType = MsgTypes.Daily,
                            Message = s,
                            ExecuteTime = executeTime
                        };
                        outgoingMsgList.Add(outgoingMsg);
                    }
                });
            });
            return outgoingMsgList;
        }


    }
}
