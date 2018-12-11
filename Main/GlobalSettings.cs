using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MUT
{
    public class Protocol
    {
        public static String encrPassword = @"jiq91lpqA5aZ";

        public String DDLFilename { get; set; }

        public String Name { get; set; }

        public String EncryptedPassword { get; set; }
        public String Password { get => Crypto.DecryptStringAES(EncryptedPassword, encrPassword); }

        public String EncryptedLogin { get; set; }
        public String Login { get=> Crypto.DecryptStringAES(EncryptedLogin, encrPassword);  }

        public String EncryptedTargetUserName { get; set; }
        public String TargetUserName { get => Crypto.DecryptStringAES(EncryptedTargetUserName, encrPassword); }

        public override string ToString()
        {
            return $"Name: {Name}";
        }
    }

    public class GlobalSettings
    {
        public String SettingsURI { get; set; }
        public String SettingsPath { get; set; }
        public String UploadLogURI { get; set; }
        public String GetWanIPHost { get; set; }
        public int PingMinutes { get; set; } = 0;
        public List <Protocol> Protocols { get; set; }

        public override string ToString()
        {
            var s = $"SettingsURI : {SettingsURI}, SettingsPath: {SettingsPath}, UploadLogURI: {UploadLogURI}\n";
            Protocols.ForEach(m => s += m.ToString() + ",");
            return s;
        }
    }

    public class GlobalSettingsIO
    {
        public static readonly String SettingsName = "settings.json";
        public static GlobalSettings Load()
        {
            //FileSystemWatcher watcher = new FileSystemWatcher();
            //watcher.Path = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            using (StreamReader file = File.OpenText(SettingsName))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (GlobalSettings)serializer.Deserialize(file, typeof(GlobalSettings));
            }
        }

        public static void Save(GlobalSettings mySettings)
        {
            File.WriteAllText(SettingsName, JsonConvert.SerializeObject(mySettings, Formatting.Indented));
        }
    }
}
