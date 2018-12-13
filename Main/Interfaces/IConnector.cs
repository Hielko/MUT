using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUT
{
    public interface IConnector
    {
        String GetName();
      //  event EventHandler OnMessageReceived;
        void InitSession(GlobalSettings globalSettings,Account protocol);
        void SendMessage(String user, String msg);
    }
}
