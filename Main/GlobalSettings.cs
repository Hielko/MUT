using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MUT
{
    public class Pass
    {
        public static String encrPassword = @"jiq91lpqA5aZ";
    }

    public class TUserToAccount
    {
        public String EncryptedTUserName { get; set; }
        public String TUserName { get => Crypto.DecryptStringAES(EncryptedTUserName, Pass.encrPassword); }
        public String ProtocolName { get; set; }
    }

    public class Account
    {
        
        public String DDLFilename { get; set; }
        public String Name { get; set; }
        public Boolean Enabled { get; set; }

        public String EncryptedPassword { get; set; }
        public String Password { get => Crypto.DecryptStringAES(EncryptedPassword, Pass.encrPassword); }

        public String EncryptedLogin { get; set; }
        public String Login { get=> Crypto.DecryptStringAES(EncryptedLogin, Pass.encrPassword);  }

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

        public List <Account> Accounts { get; set; }
        public List<TUserToAccount> TUserToAccounts { get; set; }

        public override string ToString()
        {
            var s = $"SettingsURI : {SettingsURI}, SettingsPath: {SettingsPath}, UploadLogURI: {UploadLogURI}";
            s += "\nAccounts:";
            Accounts.ForEach(m => s += m.ToString() + ",");
            s += "\nTUserToAccount:";
            TUserToAccounts.ForEach(m => s += m.ToString() + ",");
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
