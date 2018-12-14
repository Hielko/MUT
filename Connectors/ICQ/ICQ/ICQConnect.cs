using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcqSharp;
using IcqSharp.Base;
using MUT;

namespace ICQ
{
    public class ICQConnect : IConnector
    {
        private int retryCount = 0;
        private Session session;
        private GlobalSettings globalSettings;
        private Account account;
        public event EventHandler<ReceivedArgs> MessageReceived;

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
                InitSession(globalSettings, account);
            }
        }

        private void OnMessageReceived(IcqSharp.Base.Message message)
        {
            MessageReceived?.Invoke(this, new ReceivedArgs { Username = message.Contact.Nickname,
                Msg = Utils.StringUtils.StripUTF(message.Text),
                //Context: message 
            });
        }

        public void InitSession(GlobalSettings globalSettings, Account account)
        {
            this.globalSettings = globalSettings;
            this.account = account;

            session = new Session(account.Login, account.Password);
            session.ConnectionError += ReConnect;
            session.Disconnected += ReConnect;
            session.Connect();
            session.Messaging.MessageReceived += OnMessageReceived;
        }

        public void SendMessage(string user, string msg)
        {
            Contact contact = session.ContactList.Contacts.Find(f => f.Nickname.Equals(user, StringComparison.CurrentCultureIgnoreCase));
            if (contact != null)
            {
                session.Messaging.Send(new IcqSharp.Base.Message(contact, MessageType.Outgoing, msg));
            }
            else
            {
                Logger.Log.Error($"User '{user}' not found.");
            }
        }
    }
}
