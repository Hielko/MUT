using System;
using MUT;

namespace Credentials
{
    class Program
    {
        static void Main(string[] args)
        {
            string s;
            string encrPassword = "jiq91lpqA5aZ";
            GlobalSettings mySettings = GlobalSettingsIO.Load();

            var p = new Protocol();
            p.Name = "ICQ";
            mySettings.Protocols.Add(p);
            Console.WriteLine("Enter Login");
            s = Console.ReadLine();
            p.EncryptedLogin = Crypto.EncryptStringAES(s, encrPassword);
            Console.WriteLine(p.EncryptedLogin);


            Console.WriteLine("Enter pw");
            s = p.EncryptedPassword = Console.ReadLine();
            p.EncryptedPassword = Crypto.EncryptStringAES(s, encrPassword);
            Console.WriteLine(p.EncryptedPassword);


            Console.WriteLine("Enter TargetUserName");
            s = p.EncryptedTargetUserName = Console.ReadLine();
            p.EncryptedTargetUserName = Crypto.EncryptStringAES(s, encrPassword);
            Console.WriteLine(p.EncryptedTargetUserName);


            Console.WriteLine("Press enter to save");
            Console.ReadLine();
            GlobalSettingsIO.Save(mySettings);

        }
    }
}
