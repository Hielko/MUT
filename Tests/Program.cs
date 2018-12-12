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
        static CommonModule commonModule;
        static Location location;

        internal static void TestReply()
        {
             using (ReplyModule replyModule = new ReplyModule( location.GetLocation(ReplyModule.Filename)))
            {
                replyModule.Config.Settings.AntiHammerSeconds = 0;
                replyModule.Config.Settings.NoReplyChance = 0;
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
            using (DailyModule dailyModule = new DailyModule(location.GetLocation(DailyModule.Filename)))
            {
                //dailyModule.Init();
                dailyModule.Config.Settings.ChanceMultiplier = 0;
                Console.WriteLine(dailyModule);

                OutgoingMsgMngr Msgs = new OutgoingMsgMngr();
                Msgs.Add(dailyModule.Generate(commonModule));
                Console.WriteLine(Msgs);
            }
        }

        static void Main(string[] args)
        {
            globalSettings = GlobalSettingsIO.Load();
            location = new Location(globalSettings.SettingsURI, globalSettings.SettingsPath);
            commonModule = new CommonModule(location.GetLocation(CommonModule.Filename));

            TestDaily();
            TestReply();

            Console.WriteLine("Tests Finisched, press a key");
            Console.ReadKey();
        }
    }
}
