using System;

namespace MUT
{
    public class ReceivedArgs : EventArgs
    {
        public String Username { get; set; }
        public String Msg { get; set; }
        public object Context { get; set; }
    }

    public interface IConnector
    {
        String GetName();
        event EventHandler<ReceivedArgs> MessageReceived;
        void InitSession(GlobalSettings globalSettings, Account account);
        void SendMessage(String user, String msg);
    }

    public class BaseConnector
    {
        protected GlobalSettings globalSettings;
        protected Account account;
        public BaseConnector(GlobalSettings globalSettings, Account account)
        {
            this.globalSettings = globalSettings;
            this.account = account;
        }
    }
}
