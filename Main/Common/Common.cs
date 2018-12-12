using MUT.Daily;
using MUT;
using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace MUT.Common
{
    public class CommonModule : AModule<CommonConfig> 
    {
        public const string Filename = "common.json";
        private ResetTimer resetTimer;
        public Boolean IsTotalSilenceDay;
        //   public event EventHandler Changed;

        public CommonModule(Location location) : base(location)
        {
            Name = "Common";
        }
    }


    public class Parse
    {
        public static String DoParse(String text, Dictionary<String, String> dictionary)
        {
            if (String.IsNullOrEmpty(text))
                return text;

            StringBuilder stringBuilder = new StringBuilder(text);
            foreach (var keyValue in dictionary)
            {
                // todo: regex
                stringBuilder.Replace(keyValue.Key, keyValue.Value);
            }
            return stringBuilder.ToString();
        }
    }
}
