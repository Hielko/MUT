using MUT;
using MUT.Common;
using System;
using System.Collections.Generic;
using System.Timers;
using Utils;

namespace MUT.Reply
{
    public enum ReplyStatuses
    {
        Ok,
        NoMatch,
        InBlockPeriod,
        ClearedAtRandom,
        Hammering,
        Copy,
        Disabled
    }

    public class ReplyModule : AModule<ReplyConfig> 
    {
        private Timer tmrReload;
        public event EventHandler Changed;
        private String LastOutText = null;

        public override string ToString()
        {
            return String.Format("ReplyModule: {0}", config);
        }

        public override void LoadConfig()
        {
            var tmp = ConfigBase<ReplyConfig>.LoadConfig(URI, out Boolean isChanged);
            if (isChanged)
            {
                config = tmp;
                Changed?.Invoke(this, null);
            }
        }

        private void InitTimer()
        {
            tmrReload?.Dispose();
            tmrReload = new Timer
            {
                Interval = config.Settings.ReloadMinutes * (60 * 1000),
                Enabled = true
            };
            tmrReload.Elapsed += delegate (Object o, ElapsedEventArgs e)
            {
                LoadConfig();
            };
            tmrReload.Start();
        }

        public override void Init(String URI, string path)
        {
            this.URI = URI + path + "reply.json";
            LoadConfig();
            if (config.Settings.ReloadMinutes > 0)
            {
                InitTimer();
            }
        }

        private Boolean IsChance(int value)
        {
            return value > 0 && Utils.Common.Random(value) == 0;
        }

        public ReplyStatuses GenerateReply(CommonModule commonModule, String text, List<OutgoingMsg> outgoingMsgList)
        {
            String OutText = "";
            MsgOrigins msgOrigin = MsgOrigins.None;

            //
            if (!config.Settings.Enabled)
            {
                return ReplyStatuses.Disabled;
            }

            //
            if (AntiHammer.HammerDetected(config.Settings.AntiHammerSeconds))
            {
                return ReplyStatuses.Hammering;
            }

            //
            var blockPeriod = config.BlockPeriods.Find(period => period.IsMatch(DateTime.Now));
            if (blockPeriod != null)
            {
                return ReplyStatuses.InBlockPeriod;
            }

            //
            if (IsChance(config.Settings.NoReplyChance))
            {
                return ReplyStatuses.ClearedAtRandom;
            }

            var ruleGroup = config.replies.Find(reply => reply.IsMatch(text));
            if (ruleGroup != null)
            {
                OutText = ruleGroup.ReplyText;
                msgOrigin = MsgOrigins.Match;
            }
            else
            {
                var defaultReply = config.defaults.Find(dr => Utils.Common.Random(dr.Chance) == 0);
                if (defaultReply != null)
                {
                    OutText = defaultReply.ReplyText;
                    msgOrigin = MsgOrigins.Default;
                }
                else
                {
                    return ReplyStatuses.NoMatch;
                }
            }

            if (StringUtils.IsAllUppercase(text) && text.Length > config.specials.allcapsreply.MinLength)
            {
                if (IsChance(config.specials.allcapsreply.Chance))
                {
                    OutText = config.specials.allcapsreply.ReplyText;
                    msgOrigin = MsgOrigins.Special;
                }
            }

            
            OutText = Parse.DoParse(OutText, commonModule.config.variables);

            if (config.Settings.TypoChance > 0 && Utils.Common.Random(config.Settings.TypoChance) == 0) { 
                OutText = Typo.MakeTypo(Utils.StringUtils.ReplaceRandomTokens(OutText));
            }

            //
            if (OutText.ToUpper().Equals(LastOutText))
            {
                return ReplyStatuses.Copy;
            }
            LastOutText = OutText.ToUpper();

            int timeOffsetSeconds = config.Settings.DelaySeconds;
            if (msgOrigin == MsgOrigins.Default)
            {
                timeOffsetSeconds += config.Settings.DelayDefaultReplySeconds;
            }
            DateTime executeTime = DateTime.Now.AddSeconds(timeOffsetSeconds + Utils.Common.Random(60));
            foreach (var s in OutText.Split('\n'))
            {
                executeTime = executeTime.AddSeconds(5 + Utils.Common.Random(s.Length));
                outgoingMsgList.Add(new OutgoingMsg
                {
                    MsgType = MsgTypes.Reply,
                    MsgOrigin = msgOrigin,
                    Message = s,
                    ExecuteTime = executeTime
                });
            }

            if (IsChance(config.Settings.ALLCAPSRepeatChance))
            {
                if (outgoingMsgList.Count == 1 && StringUtils.IsAllUppercase(outgoingMsgList[0].Message) == false)
                {
                    outgoingMsgList.Add(new OutgoingMsg
                    {
                        MsgType = MsgTypes.Reply,
                        MsgOrigin = msgOrigin,
                        Message = outgoingMsgList[0].Message.ToUpper(),
                        ExecuteTime = executeTime.AddSeconds(Utils.Common.Random(10) + 5)
                    });
                }
            }

            return ReplyStatuses.Ok;
        }

        public override void Dispose()
        {
            tmrReload?.Stop();
        }
    }

}
