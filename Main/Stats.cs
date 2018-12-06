using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using IcqSharp.Base;

namespace Stats
{

    public class MyStats
    {
        private readonly String FileName = "stats.json";

        public MyStats()
        {
            Load();
        }

        private String MakeKey(DateTime d)
        {
            return d.ToShortDateString();
        }

        public Dictionary<String, int> stats = new Dictionary<String, int>();
        public void AddMsg()
        {
            var key = MakeKey(DateTime.Now);
            if (stats.ContainsKey(key))
            {
                stats[key]++;
            }
            else
            {
                stats.Add(key, 1);
            }
            Save();
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(FileName, JsonConvert.SerializeObject(this));
            }
            catch (Exception)
            {
             //   throw;
            }
        }

        public void Load()
        {
            try
            {
                String s = File.ReadAllText(FileName);
                MyStats m = JsonConvert.DeserializeObject<MyStats>(s);
                foreach (var kp in m.stats)
                {
                    this.stats.Add(kp.Key, kp.Value);
                }
            }
            catch (Exception)
            {
              //  throw;
            }
        }

        ~MyStats()
        {
            Save();
        }
    }


    public interface StatsInf
    {
        String GetKey();
    }


    public class DataCounter<T> where T : StatsInf
    {
        private String FileName = "stats.json";
        public Dictionary<String, int> stats = new Dictionary<String, int>();
        public void AddMsg(T t)
        {
            var key = t.GetKey();
            if (stats.ContainsKey(key))
            {
                stats[key]++;
            }
            else
            {
                stats.Add(key, 1);
            }
        }

        public void Save()
        {
            File.WriteAllText(FileName, JsonConvert.SerializeObject(this));
        }

        public void Load()
        {
            try
            {
                String s = File.ReadAllText(FileName);
                DataCounter<T> m = JsonConvert.DeserializeObject<DataCounter<T>>(s);
                foreach (var kp in m.stats)
                {
                    this.stats.Add(kp.Key, kp.Value);
                }
            }
            catch (Exception)
            {
                //     throw;
            }
        }

        public DataCounter(String FileName)
        {
            this.FileName = FileName;
        }

        ~DataCounter()
        {
            Save();
        }
    }


    public class History
    {
        //  private List<>

        public void Add(Message message)
        {
            //message.Timestamp.
        }

    }

}


