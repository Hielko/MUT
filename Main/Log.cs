using System;
using System.IO;

namespace Logger
{
    public class Log
    {
        public static bool Enable = true;
        public static readonly String FilePath = "connectionlog.txt";
        public static event EventHandler WriteEvent;

        public static void Debug(string message, params object[] args)
        {
            Write("Debug", message, args);
        }

        public static void Debug(string message)
        {
            Debug(message, null);
        }
        public static void Debug(Exception e)
        {
            Debug(e.ToString());
        }

        public static void Error(string message, params object[] args)
        {
            Write("ERROR", message, args);
        }

        public static void Error(string message)
        {
            Error(message, null);
        }

        public static void Error(Exception e)
        {
            Error(e.ToString());
        }

        public static void Warn(string message, params object[] args)
        {
            Write("Warn", message, args);
        }

        public static void Warn(Exception e)
        {
            Warn(e.ToString());
        }

        public static void Info(string message, params object[] args)
        {
            Write("Info", message, args);
        }

        public static void Info(string message)
        {
            Info(message, null);
        }

        public static void Info(Exception e)
        {
            Warn(e.ToString());
        }

        private static void Write(string type, string message, params object[] args)
        {
            if (!Enable)
                return;

            string s;

            if (args == null)
                s = String.Format("{0:MMddyy HH:mm:ss}: {1} {2}", DateTime.Now, type, message);
            else
                s = String.Format("{0:MMddyy HH:mm:ss}: {1} {2} {3}", DateTime.Now, type, message, args);

            Console.WriteLine(s);

            using (StreamWriter streamWriter = new StreamWriter(FilePath, true))
            {
                streamWriter.WriteLine(s);
                streamWriter.Close();
            }

            WriteEvent?.Invoke(s, EventArgs.Empty);
        }
    }
}
