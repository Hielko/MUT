using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class TestData : IEnumerable
    {
        private List<String> content = new List<string>();
        public TestData()
        {
            string[] readText = File.ReadAllLines("testdata.txt");
            foreach (string s in readText)
            {
                content.Add(s);
            }
        }

        public IEnumerator GetEnumerator()
        {
           return content.GetEnumerator();
        }
    }
}
