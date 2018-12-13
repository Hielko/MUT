using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUT
{
    public class ReceivedArgs : EventArgs
    {
        public String Username { get; set; }
        public String Msg { get; set; }
    }

    public interface IConnector
    {
        String GetName();
        event EventHandler<ReceivedArgs> MessageReceived;
        void InitSession(GlobalSettings globalSettings,Account account);
        void SendMessage(String user, String msg);
    }
}
