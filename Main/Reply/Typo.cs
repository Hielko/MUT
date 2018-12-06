using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MUT.Reply
{
    class Typo
    {
        static char ToggleCase(char c)
        {
            if (Char.IsUpper(c))
                return Char.ToLower(c);
            if (Char.IsLower(c))
                return Char.ToUpper(c);
            return c;
        }

        static string ReplaceAt(string input, int index, char newChar)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            char[] chars = input.ToCharArray();
            chars[index] = newChar;
            return new string(chars);
        }

        public static string MakeTypo(string s)
        {
            if (String.IsNullOrEmpty(s))
                return s;

            var result = s;
            int length = new Random(Guid.NewGuid().GetHashCode()).Next(s.Length);
            switch (new Random(Guid.NewGuid().GetHashCode()).Next(4))
            {
                case 0: result = result.Remove(length, 1); break;
                case 1: result = result.Insert(length, " "); break;
                case 2: result = result.Insert(length, result[length].ToString()); break;
                case 3: result = ReplaceAt(result, length, ToggleCase(result[length])); break;
            }
            return result;
        }
       
    }
}
