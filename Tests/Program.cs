using System;
using System.Collections.Generic;
using MUT;
using MUT.Daily;
using MUT.Common;
using MUT.Reply;

namespace Tests
{
    class Program
    {
        static GlobalSettings globalSettings = new GlobalSettings();
        static CommonModule commonModule = new CommonModule();

        internal static void TestReply()
        {
            using (ReplyModule replyModule = new ReplyModule())
            {
                replyModule.Init(globalSettings.SettingsURI, globalSettings.SettingsPath);
                replyModule.config.Settings.AntiHammerSeconds = 0;
                replyModule.config.Settings.NoReplyChance = 0;
                //     replyModule.config.Settings.ALLCAPSRepeatChance = 7;
                Console.WriteLine(replyModule);
                foreach (string sentence in new TestData())
                {
                    List<OutgoingMsg> Msgs = new List<OutgoingMsg>();
                    var reply = replyModule.GenerateReply(commonModule, sentence, Msgs);
                    System.Console.WriteLine($"\nIn: {sentence}: Status: {reply}");
                    Msgs.ForEach(m => System.Console.WriteLine("   OUT: " + m));
                    System.Console.WriteLine(" -------------------------- ");
                }
            }
        }

        internal static void TestDaily()
        {
            using (DailyModule dailyModule = new DailyModule())
            {
                dailyModule.Init(globalSettings.SettingsURI, globalSettings.SettingsPath);
                dailyModule.config.Settings.ChanceMultiplier = 0;
                Console.WriteLine(dailyModule);

                OutgoingMsgMngr Msgs = new OutgoingMsgMngr();
                Msgs.Add(dailyModule.Generate(commonModule));
                Console.WriteLine(Msgs);
            }
        }

        static void Main(string[] args)
        {
            globalSettings = GlobalSettingsIO.Load();
            commonModule.Init(globalSettings.SettingsURI, globalSettings.SettingsPath);

            TestDaily();
            TestReply();

            Console.WriteLine("Tests Finisched, press a key");
            Console.ReadKey();
        }
    }
}
