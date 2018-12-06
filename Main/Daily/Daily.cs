using System;
using System.Collections.Generic;
using System.Linq;
using MUT.Common;
using Utils;

namespace MUT.Daily
{
    public class ResetDailyEventArgs : EventArgs
    {
        public Boolean ByReset;
    }

    public class DailyModule : AModule<DailyConfig>
    {
        private ResetTimer resetTimer;
//        private System.Timers.Timer tmrReload;
        public event EventHandler<ResetDailyEventArgs> Loaded;

        public override string ToString()
        {
            return String.Format("DailyModule: {0}", config);
        }

        public override void LoadConfig()
        {
            config = ConfigBase<MUT.Daily.DailyConfig>.LoadConfig(URI, out Boolean isChanged);
        }
        /*
        private void InitTimer()
        {
            if (config.Settings.resetTime == null)
                return;

            var resetTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, config.Settings.resetTime.Hour, config.Settings.resetTime.Minute, 0);
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
                LoadConfig();
                Loaded?.Invoke(this, new ResetDailyEventArgs { ByReset = true });
                InitTimer();
            };
            tmrReload.Start();
        }
        */
        public override void Init(String URI, string path)
        {
            this.URI = URI + path + "daily.json";
            LoadConfig();
            Loaded?.Invoke(this, new ResetDailyEventArgs { ByReset = false });
            resetTimer = new ResetTimer(config.Settings.resetTime, delegate ()
           {
               LoadConfig();
               Loaded?.Invoke(this, new ResetDailyEventArgs { ByReset = true });
               return 0;
           }

                );
           // InitTimer();
        }

        public List<OutgoingMsg> Generate(CommonModule commonModule)
        {
            var outgoingMsgList = new List<OutgoingMsg>();

            if (!config.Settings.Enabled)
            {
                return outgoingMsgList;
            }

            List<MessageGroup> messageGroups = config.Messages.FindAll(mg => mg.DaysList.Count == 0 || mg.DaysList.Exists(dayOfWeek => dayOfWeek.Equals(DateTime.Now.DayOfWeek)));
            messageGroups = messageGroups.FindAll(mg => Utils.Common.Random(mg.Chance * config.Settings.ChanceMultiplier) == 0);

            messageGroups.ForEach(delegate (MessageGroup messageGroup)
            {
                DateTime executeTime = messageGroup.BotTime.FromTime.AddMinutes(Utils.Common.Random((int)messageGroup.BotTime.TotalMinutes));
                var msgs = messageGroup.Msgs.ToList().FindAll(mg => Utils.Common.Random(mg.Chance) == 0);

                msgs.ForEach(delegate (Message message)
                {
                    var text = Parse.DoParse(message.Text, commonModule.config.variables);
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

        public override void Dispose()
        {
          //  tmrReload?.Stop();
        }
    }
}
