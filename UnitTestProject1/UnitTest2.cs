using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MUT.Reply;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            string[] c = { "1", "2","a" };
            RuleGroup r = new RuleGroup { Contains = c };
            Assert.IsTrue(r.IsMatch("1"), "Contains 1");
            Assert.IsTrue(r.IsMatch("2"), "Contains 2");
            Assert.IsTrue(r.IsMatch("a"), "Contains a");
            Assert.IsTrue(r.IsMatch("A"), "Contains A");
            Assert.IsFalse(r.IsMatch("3"), "not Contains 3");


            string[] startwith = { "this", "is", "a", "centence" };
            r = new RuleGroup { Startswith = startwith };
            Assert.IsTrue(r.IsMatch("this is a centence"), "Startswith 'this'");
            Assert.IsTrue(r.IsMatch("is this a centence"), "Startswith 'is' ");
            Assert.IsFalse(r.IsMatch("none"), "Startswith not with 'none' ");

        }
    }
}
