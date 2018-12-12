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
        public const string Filename = "reply.json";
        private Timer tmrReload;
        private string LastOutText = null;

        private void InitTimer()
        {
            tmrReload?.Dispose();
            tmrReload = new Timer
            {
                Interval = Config.Settings.ReloadMinutes * (60 * 1000),
                Enabled = true
            };
            tmrReload.Elapsed += delegate (Object o, ElapsedEventArgs e)
            {
                LoadConfig();
            };
            tmrReload.Start();
        }

        public ReplyModule(Location location) : base(location)
        {
            Name = "Replies";
            if (Config.Settings.ReloadMinutes > 0)
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
            if (!Config.Settings.Enabled)
            {
                return ReplyStatuses.Disabled;
            }

            //
            if (AntiHammer.HammerDetected(Config.Settings.AntiHammerSeconds))
            {
                return ReplyStatuses.Hammering;
            }

            //
            var blockPeriod = Config.BlockPeriods.Find(period => period.IsMatch(DateTime.Now));
            if (blockPeriod != null)
            {
                return ReplyStatuses.InBlockPeriod;
            }

            //
            if (IsChance(Config.Settings.NoReplyChance))
            {
                return ReplyStatuses.ClearedAtRandom;
            }

            var ruleGroup = Config.replies.Find(reply => reply.IsMatch(text));
            if (ruleGroup != null)
            {
                OutText = ruleGroup.ReplyText;
                msgOrigin = MsgOrigins.Match;
            }
            else
            {
                var defaultReply = Config.defaults.Find(dr => Utils.Common.Random(dr.Chance) == 0);
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

            if (StringUtils.IsAllUppercase(text) && text.Length > Config.specials.allcapsreply.MinLength)
            {
                if (IsChance(Config.specials.allcapsreply.Chance))
                {
                    OutText = Config.specials.allcapsreply.ReplyText;
                    msgOrigin = MsgOrigins.Special;
                }
            }

            
            OutText = Parse.DoParse(OutText, commonModule.Config.variables);
            OutText = Utils.StringUtils.ReplaceRandomTokens(OutText);

            if (Config.Settings.TypoChance > 0 && Utils.Common.Random(Config.Settings.TypoChance) == 0) { 
                OutText = Typo.MakeTypo(OutText);
            }

            //
            if (OutText.ToUpper().Equals(LastOutText))
            {
                return ReplyStatuses.Copy;
            }
            LastOutText = OutText.ToUpper();

            int timeOffsetSeconds = Config.Settings.DelaySeconds;
            if (msgOrigin == MsgOrigins.Default)
            {
                timeOffsetSeconds += Config.Settings.DelayDefaultReplySeconds;
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

            if (IsChance(Config.Settings.ALLCAPSRepeatChance))
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


    }

}
