using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MUT.Reply;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestReplaceBlockPeriod()
        {
         
            if (true)
            {
                BlockPeriod p = new BlockPeriod { Time = "11:30-12:30" };
                Assert.IsTrue(p.IsMatch(new DateTime(2000, 8, 1, 11, 31, 0)), "2001, 8, 1, 11, 31");
                Assert.IsFalse(p.IsMatch(new DateTime(2000, 8, 1, 11, 29, 0)), "2002, 8, 1, 11, 29");
            }

            {
                BlockPeriod p = new BlockPeriod { Time = "23:30-02:30" };
                Assert.IsTrue(p.IsMatch(new DateTime(2000, 8, 1, 23, 31, 0)), p.ToString() + " 2003, 8, 1, 11, 31");
                Assert.IsTrue(p.IsMatch(new DateTime(2006, 8, 1, 1, 1, 0)), p.ToString() + "  2004, 8, 1, 1, 1");
                Assert.IsFalse(p.IsMatch(new DateTime(2000, 9, 1, 2, 31, 0)), p.ToString() + " 2005, 9, 1, 2, 31");
            }

            {
                BlockPeriod p = new BlockPeriod { Days = "Saturday" };
                Assert.IsTrue(p.IsMatch(new DateTime(2018, 9, 1, 23, 31, 0)), p.ToString() + " is Saterday");
                p.Time = "12:00-13:00";
                Assert.IsFalse(p.IsMatch(new DateTime(2018, 9, 2, 12, 0, 0)), p.ToString() + " is Saterday not in timeperiode");
            }

            {
                BlockPeriod p = new BlockPeriod { Time = "01:00-11:00", Days = "Monday,Tuesday,Wednesday,Thursday,Friday" };
                Assert.IsTrue(p.IsMatch(new DateTime(2018, 8, 30, 10, 59, 0)), p.ToString() + " zzzzzzzzzzz");
            }

    
            {
                BlockPeriod p = new BlockPeriod { Time = "23:30-06:00", Days = "Monday" };
                Assert.IsTrue(p.IsMatch(new DateTime(2018, 8, 27, 23, 50, 0)), p.ToString() + " Block ");
                Assert.IsTrue(p.IsMatch(new DateTime(2018, 8, 27, 0, 50, 0)), p.ToString() + " Block ");
            }

        }


        [TestMethod]
        public void TestReplaceRandomTokens()
        {

            {
                string result = Utils.StringUtils.ReplaceRandomTokens("{1}");
                //Assert.IsTrue(("1", result);
            }

            {
                string result = Utils.StringUtils.ReplaceRandomTokens("{}");
                Assert.AreEqual("", result);
            }
            {
                string result = Utils.StringUtils.ReplaceRandomTokens("{1,2}");
                if (!(result.Equals("1") || result.Equals("2")))
                {
                    Assert.Fail("must be 1 or 2");
                }
            }

            {
                string result = Utils.StringUtils.ReplaceRandomTokens("{1,}");
                if (!(result.Equals("1") || result.Equals("")))
                {
                    Assert.Fail("must be 1 or empty");
                }
            }

        }
    }
}
