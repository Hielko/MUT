using IcqSharp;
using System;
using System.Collections.Generic;
using System.Threading;
using Logger;
using System.Net;
using IcqSharp.Base;
using System.Timers;
using System.Text.RegularExpressions;
using MUT.Daily;
using MUT.Common;
using MUT.Reply;

namespace MUT
{
    public class Program
    {
        static int retryCount = 0;
        static Session session;
        static GlobalSettings globalSettings;
        static OutgoingMsgMngr outgoingMsgMngr = new OutgoingMsgMngr();
        static ReplyModule replyModule;
        static DailyModule dailyModule;
        static CommonModule commonModule;
        static Location location;
        //static MyStats Stats = new MyStats();
        static Thread ThrProcessOutgoingMsgList;
        static string encrPassword = "jiq91lpqA5aZ";
        static string targetUserName;

        private static void ReConnect(object o, EventArgs args)
        {
            Log.Error("ReConnect");
            retryCount++;
            if (retryCount < 1000)
            {
                System.Threading.Thread.Sleep(30000);
                session.Dispose();
                InitSession();
            }
        }

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

        private static void OnMessageReceived(IcqSharp.Base.Message message)
        {
            if (message.Contact.Nickname.Equals(targetUserName, StringComparison.CurrentCultureIgnoreCase))
            {
                //  Stats.AddMsg();
                Log.Debug(String.Format("{0} {1}", "Receiving ", message.Text));
                var logStr="";

                if (commonModule.IsTotalSilenceDay)
                {
                    logStr = "IsTotalSilenceDay";
                }
                else
                {
                    List<OutgoingMsg> Msgs = new List<OutgoingMsg>();
                    var reply = replyModule.GenerateReply(commonModule, Utils.StringUtils.StripUTF(Regex.Replace(message.Text, @"\t|\n|\r", " ")), Msgs);
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

                Log.Debug(logStr+"\n");
            }
        }

        private static void InitSession()
        {
            String password = Crypto.DecryptStringAES(globalSettings.EncryptedPassword, encrPassword);
            String UIN = Crypto.DecryptStringAES(globalSettings.EncryptedLogin, encrPassword);
            session = new Session(UIN, password);
            session.ConnectionError += ReConnect;
            session.Disconnected += ReConnect;
            session.Connect();
            session.Messaging.MessageReceived += OnMessageReceived;
        }

        private static void InitCommon()
        {
            try
            {
                commonModule = new CommonModule(location.GetLocation(CommonModule.Filename));
                /*
                commonModule.Changed += delegate (object o, EventArgs e)
                {
                    Log.Info("commonModule: re-loaded: " + commonModule);
                };
                */
             //   commonModule.Init(globalSettings.SettingsURI, globalSettings.SettingsPath);
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
                replyModule.configBase.Changed += delegate (object o, EventArgs e)
                {
                    Log.Info("replyModule: re-loaded: " + replyModule);
                };
               // replyModule.Init(globalSettings.SettingsURI ,globalSettings.SettingsPath);
                Log.Info("replyModule: " + replyModule);
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
                dailyModule.Loaded += delegate (object o, ResetDailyEventArgs e)
                {
                    outgoingMsgMngr.Clear();
                    if (e.ByReset || !outgoingMsgMngr.ResumeFileExists)
                    {
                        outgoingMsgMngr.Add(dailyModule.Generate(commonModule));
                        outgoingMsgMngr.Save();
                        Log.Info("New Daily: " + outgoingMsgMngr);
                    }
                    else
                    {
                        outgoingMsgMngr.Load();
                        Log.Info("Resume Daily: " + outgoingMsgMngr);
                    }
                };
             //   dailyModule.Init(globalSettings.SettingsURI, globalSettings.SettingsPath);
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
                Contact contact = session.ContactList.Contacts.Find(c => c.Nickname.Equals(targetUserName, StringComparison.CurrentCultureIgnoreCase));
                if (contact != null)
                {
                    OutgoingMsg msg = outgoingMsgMngr.PopFirstExpired();
                    if (msg != null)
                    {
                        if (commonModule.IsTotalSilenceDay)
                        {
                            Log.Debug("IsTotalSilenceDay: Not sending  " + msg.Message);
                        }
                        else
                        {
                            Log.Debug("Sending  " + msg.Message);
                            session.Messaging.Send(new IcqSharp.Base.Message(contact, MessageType.Incoming, msg.Message));
                        }
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
                String str = String.Format("Alive: Connected={0} ", session.IsConnected.ToString());
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

            targetUserName = Crypto.DecryptStringAES(globalSettings.EncryptedTargetUserName, encrPassword);

            InitCommon();
            InitReplies();
            InitDaily();
            
            InitSession();

            if (globalSettings.PingMinutes > 0)
            {
                InitPing();
            }

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
