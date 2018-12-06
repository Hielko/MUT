using Newtonsoft.Json;
using System;
using System.Net;

namespace MUT
{
    public abstract class AModule<T> : IDisposable
    {
        public T config;
        public String URI { get; set; }
        public abstract void Dispose();
        public abstract void LoadConfig();
        public abstract void Init(String URI, string path);
    }


    public static class ConfigBase<T>
    {
        private static String prevSource;

        public static T LoadConfig(String URI, out Boolean isChanged)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string pageSource = client.DownloadString(URI);
                    isChanged = !pageSource.Equals(prevSource);
                    if (isChanged)
                    {
                    //    TESTChanged?.Invoke(T, null);
                    }
                    prevSource = pageSource;
                    return JsonConvert.DeserializeObject<T>(pageSource);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error($"Error loading URI {URI}: {ex}");
                //Console.WriteLine(String.Format("Error loading URI: {0}",URI));
                //Console.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
