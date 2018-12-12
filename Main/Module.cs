using Newtonsoft.Json;
using System;
using System.Net;
using Logger;

namespace MUT
{
    public class Location
    {
        private String URI { get; set; }
        private String Path { get; set; }
        private String Filename { get; set; }
        public Location(String pURI, string pPath, string pFilename)
        {
            this.URI = pURI;
            this.Path = pPath;
            this.Filename = pFilename;
        }
        public Location(String pURI, string pPath)
        {
            this.URI = pURI;
            this.Path = pPath;
        }
        public String GetLocation()
        {
            return this.URI + this.Path + this.Filename;
        }
        public Location GetLocation(String Filename)
        {
            return new Location(this.URI, this.Path, Filename);
        }
    }


    public class AModule<T> : IDisposable
    {
        public String Name { get; protected set; }
        public T Config { get; protected set; }
        public ConfigBase<T> configBase { get; protected set; }
        public Location Location { get; private set; }
        public event EventHandler Loaded;
        public AModule(Location pLocation)
        {
            this.Location = pLocation;
            configBase = new ConfigBase<T>();
            LoadConfig();
            configBase.Changed += delegate (object o, EventArgs e)
            {
                Log.Info("Changed: " + ToString());
            };
        }

        public void LoadConfig()
        {
            Config = configBase.LoadConfig(Location.GetLocation());
            Loaded?.Invoke(this, null);
        }

        public void Dispose()
        {
            //  throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{Name}: {Config}";
        }
    }


    public class ConfigBase<T>
    {
        private String prevSource;
        public event EventHandler Changed;

        public T LoadConfig(String pURI)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string pageSource = client.DownloadString(pURI);
                    if (!pageSource.Equals(prevSource))
                    {
                        Changed?.Invoke(this, null);
                    }
                    prevSource = pageSource;
                    return JsonConvert.DeserializeObject<T>(pageSource);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading URI {pURI}: {ex}");
                Logger.Log.Error($"Error loading URI {pURI}: {ex}");
                throw;
            }
        }
    }
}
