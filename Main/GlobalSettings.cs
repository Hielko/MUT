using Newtonsoft.Json;
using System;
using System.IO;

namespace MUT
{
    public class GlobalSettings
    {
        public String SettingsURI { get; set; }
        public String SettingsPath { get; set; }
        public String EncryptedPassword { get; set; }
        public String EncryptedLogin { get; set; }
        public String EncryptedTargetUserName { get; set; }
        public String UploadLogURI { get; set; }
        public String GetWanIPHost { get; set; }
        public int PingMinutes { get; set; } = 0;

        public override string ToString()
        {
            return $"SettingsURI : {SettingsURI}, SettingsPath: {SettingsPath}, UploadLogURI: {UploadLogURI}";
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
