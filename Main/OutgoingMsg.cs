using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public enum MsgTypes  { Reply, Daily };
public enum MsgOrigins { None, Match, Default, Special };

public class OutgoingMsg
{
    public DateTime ExecuteTime { set; get; }
    public String Message { set; get; }
    public MsgTypes MsgType { set; get; }
    public MsgOrigins MsgOrigin { set; get; }
    public override string ToString()
    {
        return String.Format("t={0:HH:mm:ss} : {1}", ExecuteTime, Message);
    }
}

// should be singeton
public class OutgoingMsgMngr : IEnumerable
{
    private const String Filename = "outgoing.json";
    private List<OutgoingMsg> Msgs = new List<OutgoingMsg>();
     
    public bool ResumeFileExists  { get => File.Exists(Filename); }

    public int CancelDefaultMsgs()
    {
        if (Msgs.Count > 0)
        {
            var last = Msgs.Last();
            if (last.MsgOrigin == MsgOrigins.Match || last.MsgOrigin == MsgOrigins.Special)
            {
                return Msgs.RemoveAll(x => x.MsgOrigin == MsgOrigins.Default && x.MsgType==MsgTypes.Reply);
            }
        }
        return 0;
    }

    internal Boolean CheckMessage(String text)
    {
        if (text.Contains("$")) return false;
        if (text.Contains("{")) return false;
        if (text.Contains("}")) return false;
        return true;
    }

    public void Add(OutgoingMsg outgoingMsg)
    {
        if (CheckMessage(outgoingMsg.Message))
        {
            Msgs.Add(outgoingMsg);
        }
    }

    public void Add(List<OutgoingMsg> o)
    {
        o.ForEach(n => Add(n));
        Msgs = RemoveExpired();
    }

    public void Clear()
    {
        Msgs.Clear();
    }

    public void Remove(OutgoingMsg o)
    {
        Msgs.Remove(o);
    }

    public List<OutgoingMsg> RemoveExpired()
    {
        return Msgs.FindAll(n => n.ExecuteTime > DateTime.Now); 
    }

    public override string ToString()
    {
        var s = String.Format("OutgoingMsgMngr count={0}:\n", Msgs.Count);
        Msgs.OrderBy(n => n.ExecuteTime).ToList().ForEach(outgoingMsg =>
        {
            s += "    " + outgoingMsg.ToString() + "\n";
        });
        return s;
    }

    public IEnumerator GetEnumerator()
    {
        return Msgs.OrderBy(n => n.ExecuteTime).Where(n => n.ExecuteTime > DateTime.Now).GetEnumerator();
    }

    public OutgoingMsg PopFirstExpired()
    {
        var msg = Msgs.OrderBy(n => n.ExecuteTime).Where(n => DateTime.Now > n.ExecuteTime).FirstOrDefault();
        Remove(msg);
        return msg;
    }

    public void Save()
    {
        File.WriteAllText(Filename, JsonConvert.SerializeObject(Msgs, Formatting.Indented));
    }

    public void Delete()
    {
        File.Delete(Filename);
    }



    public Boolean Load()
    {
        if (ResumeFileExists)
        {
            try
            {
                using (StreamReader file = File.OpenText(Filename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    Msgs = ((List<OutgoingMsg>)serializer.Deserialize(file, typeof(List<OutgoingMsg>)));
                    Msgs = RemoveExpired();
                }
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
