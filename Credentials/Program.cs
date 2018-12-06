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

            Console.WriteLine("Enter Login");
            s = Console.ReadLine();
            mySettings.EncryptedLogin = Crypto.EncryptStringAES(s, encrPassword);
            Console.WriteLine(mySettings.EncryptedLogin);


            Console.WriteLine("Enter pw");
            s = mySettings.EncryptedPassword = Console.ReadLine();
            mySettings.EncryptedPassword = Crypto.EncryptStringAES(s, encrPassword);
            Console.WriteLine(mySettings.EncryptedPassword);


            Console.WriteLine("Enter TargetUserName");
            s = mySettings.EncryptedTargetUserName = Console.ReadLine();
            mySettings.EncryptedTargetUserName = Crypto.EncryptStringAES(s, encrPassword);
            Console.WriteLine(mySettings.EncryptedTargetUserName);


            Console.WriteLine("Press enter to save");
            Console.ReadLine();
            GlobalSettingsIO.Save(mySettings);

        }
    }
}
