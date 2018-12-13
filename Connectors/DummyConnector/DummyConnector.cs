using MUT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace DummyConnector
{
    public class Dummy : IConnector
    {
        private Timer t;

        public event EventHandler<ReceivedArgs> MessageReceived;

        public string GetName()
        {
            return "Dummy";
        }

        public void InitSession(GlobalSettings globalSettings, Account account)
        {
            t = new System.Timers.Timer
            {
                Interval = 2000,
                AutoReset = true,
                Enabled = true
            };
            t.Elapsed += delegate (Object o, ElapsedEventArgs e)
            {
                ReceivedArgs ra = new ReceivedArgs { Username = "Username1", Msg = "Msg1" };
                MessageReceived?.Invoke(this, ra);
            };
            t.Start();
        }

        public void SendMessage(string user, string msg)
        {
            Console.WriteLine($"{GetName()} : {user}:{msg}");
        }
    }
}
