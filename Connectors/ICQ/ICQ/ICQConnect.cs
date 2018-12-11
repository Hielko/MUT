using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcqSharp;
using MUT;

namespace ICQ
{
    public class ICQConnect : IConnector
    {
        private int retryCount = 0;
        private Session session;
        private GlobalSettings globalSettings;
        private Protocol protocol;

        public string GetName()
        {
            return "ICQ";
        }

        private void ReConnect(object o, EventArgs args)
        {
            Logger.Log.Error("ReConnect");
            retryCount++;
            if (retryCount < 1000)
            {
                System.Threading.Thread.Sleep(30000);
                session.Dispose();
                InitSession(globalSettings, protocol);
            }
        }


        private void MessageReceived(IcqSharp.Base.Message message)
        {
            //   OnMessageReceived?.Invoke(this, null);
        }

        public ICQConnect()
        {
            //GlobalSettings globalSettings, Protocol protocol
        }


        public void InitSession(GlobalSettings globalSettings, Protocol protocol)
        {
            this.globalSettings = globalSettings;
            this.protocol = protocol;

            session = new Session(protocol.Login, protocol.Password);
            session.ConnectionError += ReConnect;
            session.Disconnected += ReConnect;
            session.Connect();
            session.Messaging.MessageReceived += MessageReceived;
        }

        public void SendMessage(string user, string msg)
        {
            // OnMessageReceived?.Invoke(this, null);
        }
    }
}
