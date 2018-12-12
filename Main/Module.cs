using Newtonsoft.Json;
using System;
using System.Net;

namespace MUT
{
    public class Location
    {
        private String URI { get; set; }
        private String Path { get; set; }
        private String Filename { get; set; }
        public Location(String URI, string path, string filename)
        {
            this.URI = URI;
            this.Path = path;
            this.Filename = filename;
        }
        public Location(String URI, string path)
        {
            this.URI = URI;
            this.Path = path;
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
        public AModule(Location pLocation)
        {
            this.Location = pLocation;
            configBase = new ConfigBase<T>();
            LoadConfig();
        }
        public void LoadConfig()
        {
            Console.WriteLine(Location.GetLocation());
            Config = configBase.LoadConfig(Location.GetLocation());
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

        public T LoadConfig(String URI)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    string pageSource = client.DownloadString(URI);
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
                Console.WriteLine($"Error loading URI {URI}: {ex}");
                Logger.Log.Error($"Error loading URI {URI}: {ex}");
                throw;
            }
        }
    }
}
