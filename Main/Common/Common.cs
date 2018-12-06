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
        private ResetTimer resetTimer;
        public Boolean IsTotalSilenceDay;
      //  public event EventHandler Changed;

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override void Init(string URI, string path)
        {
            this.URI = URI + path + "common.json";
            LoadConfig();
            resetTimer = new ResetTimer(config.resetTime, delegate ()
              {
                  IsTotalSilenceDay = (config.AllDisabled > 0) && (Utils.Common.Random(config.AllDisabled) == 0);
                  return 0;
              }
                );
        }

        public override void LoadConfig()
        {
            config = ConfigBase<CommonConfig>.LoadConfig(this.URI, out Boolean isChanged);
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
