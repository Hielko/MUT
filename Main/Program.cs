using System;
using System.Collections.Generic;
using System.Threading;
using Logger;
using System.Net;
using System.Timers;
using System.Text.RegularExpressions;
using MUT.Daily;
using MUT.Common;
using MUT.Reply;
using System.Linq;

namespace MUT
{
    public class Program
    {
        static GlobalSettings globalSettings;
        static OutgoingMsgMngr outgoingMsgMngr = new OutgoingMsgMngr();
        static ReplyModule replyModule;
        static DailyModule dailyModule;
        static CommonModule commonModule;
        static Location location;
        static Thread ThrProcessOutgoingMsgList;
        static ICollection<IConnector> plugins;

        private static void UploadLog(Object o, EventArgs e)
        {
            try
            {
                string myParameters = "text=" + (o as String);
                using (WebClient client = new WebClient())
                {
                    client.DownloadString(globalSettings.UploadLogURI + "?" + myParameters);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }



        private static void P_MessageReceived(object sender, ReceivedArgs e)
        {
            Log.Debug($"{sender}: {e.Username}   {e.Msg} ");

            // Utils.StringUtils.StripUTF(
            var logStr = "";
            if (commonModule.Config.AllDisabled > 0)
            {
                logStr = "AllDisabled";
            }
            else
            {
                List<OutgoingMsg> Msgs = new List<OutgoingMsg>();
                var reply = replyModule.GenerateReply(commonModule, Regex.Replace(e.Msg, @"\t|\n|\r", " "), Msgs);
                logStr = "Reply: " + reply.ToString();
                if (reply == ReplyStatuses.Ok)
                {
                    Msgs.ForEach(m => logStr += (",  Out: " + m.ToString() + "\n"));
                    outgoingMsgMngr.Add(Msgs);
                }
                var canceled = outgoingMsgMngr.CancelDefaultMsgs();
                if (canceled > 0)
                {
                    logStr += String.Format("  ** {0} canceled ** ", canceled);
                }
            }

            Log.Debug(logStr + "\n");
        }

        private static void InitCommon()
        {
            try
            {
                commonModule = new CommonModule(location.GetLocation(CommonModule.Filename));
                Log.Info(commonModule.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private static void InitReplies()
        {
            try
            {
                replyModule = new ReplyModule(location.GetLocation(ReplyModule.Filename));
                Log.Info(replyModule.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private static void InitDaily()
        {
            try
            {
                dailyModule = new DailyModule(location.GetLocation(DailyModule.Filename));
                dailyModule.Reset  += delegate (object o, EventArgs e)
                {
                    outgoingMsgMngr.Clear();
                    outgoingMsgMngr.Add(dailyModule.Generate(commonModule));
                    outgoingMsgMngr.Save();
                    Log.Info("New Generated Daily: " + outgoingMsgMngr);
                };
                Log.Info("dailyModule: " + dailyModule);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }

        private static void ProcessOutgoingMsgList()
        {
            do
            {
                Thread.Sleep(5000);
                OutgoingMsg msg = outgoingMsgMngr.PopFirstExpired();

                if (msg != null)
                {
                    if (commonModule.Config.AllDisabled>0)
                    {
                        Log.Debug("AllDisabled: Not sending  " + msg.Message);
                    }
                    else
                    {
                        Log.Debug("Sending  " + msg.Message);
                        var tm = globalSettings.TUserToAccounts.FindAll(m => m.TUserName.Equals(msg.TUserName));
                        tm.ForEach(m =>
                        {
                            var account = globalSettings.Accounts.Find(a => a.Name.Equals(m.TUserName));
                            var ps = plugins.ToList().FindAll(p => p.GetName().Equals(m.ProtocolName));
                            ps.ForEach(p => p.SendMessage(m.TUserName, msg.Message));

                        });

                        
                  //      globalSettings.TUserToAccounts.FindAll(m=>m.TUserName.Equals( msg.TUserName)).ForEach(z=>)

                        //session.Messaging.Send(new IcqSharp.Base.Message(contact, MessageType.Incoming, msg.Message));
                        //  plugins.ToList().ForEach(p => p.SendMessage(       ))

                    }
                }
            } while (true);
        }

        private static string GetWANIp()
        {
            string externalip = "unknown WAN ip";
            try
            {
                externalip = new WebClient().DownloadString(globalSettings.GetWanIPHost);
            }
            catch (Exception e)
            {
                //externalip += ": " + e.ToString();
            }
            return externalip;
        }

        private static void InitPing()
        {
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += delegate (Object o, ElapsedEventArgs e)
            {
                String str = ""; // String.Format("Alive: Connected={0} ", session.IsConnected.ToString());
                if (!String.IsNullOrEmpty(globalSettings.GetWanIPHost))
                    str += String.Format("from Wanip={0}, ", GetWANIp());
                Log.Debug(str);
            };
            aTimer.Interval = globalSettings.PingMinutes * (60 * 1000);
            aTimer.Enabled = true;
            aTimer.Start();
        }

        public static void Main(string[] args)
        {
            globalSettings = GlobalSettingsIO.Load();
            location = new Location(globalSettings.SettingsURI, globalSettings.SettingsPath);

            if (!String.IsNullOrEmpty(globalSettings.UploadLogURI))
            {
                Log.WriteEvent += UploadLog;
            }

            Log.Debug("--------------------------------------------------");
            Log.Debug($"OSVersion: {Environment.OSVersion}");
            Log.Debug("Settings: " + globalSettings);

            List<String> DDLS = new List<string>();
            globalSettings.Accounts.FindAll(z=>z.Enabled).ForEach(m => { DDLS.Add(m.DDLFilename); });
            plugins = MyPlugins<IConnector>.GetPlugins(DDLS.ToArray());

            InitCommon();
            InitReplies();
            InitDaily();

            plugins.ToList().ForEach(p => p.MessageReceived += P_MessageReceived);

            plugins.ElementAt(0).InitSession(globalSettings, globalSettings.Accounts[0]);

            if (globalSettings.PingMinutes > 0)
            {
                InitPing();
            }

            outgoingMsgMngr.Load();
            Log.Info("Resume Daily: " + outgoingMsgMngr);

            // Main send msg's loop
            ThrProcessOutgoingMsgList = new Thread(new ThreadStart(ProcessOutgoingMsgList));
            ThrProcessOutgoingMsgList.Start();

            while (true)
            {
                Thread.Sleep(60000);
            }
        }
    }
}
