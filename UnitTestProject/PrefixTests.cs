using System;
using System.Linq;
using DynamicConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class PrefixTests
    {
        private readonly PrefixConfig _prefixConfig = new PrefixConfig("DEV", "ios", "iphone", "stageB");
                                                                       
        [TestMethod]
        public void EqualityTest()
        {
            var p1 = new Prefix(_prefixConfig, "DEV", "iphone");
            var p2 = new Prefix(_prefixConfig, "DEV");
            var p3 = new Prefix(_prefixConfig, "stageB");
            var p4 = new Prefix(_prefixConfig, "DEV", "iphone");

            Assert.IsTrue(p1.CompareTo(p2) > 0);
            Assert.IsTrue(p1.CompareTo(p3) > 0);
            Assert.IsTrue(p2.CompareTo(p3) > 0);
            Assert.AreNotEqual(p1, p2);
            Assert.AreEqual(p1, p4);
        }

        [TestMethod]
        public void CompareTest()
        {
            var p1 = new Prefix(_prefixConfig, "DEV", "iphone");
            var p2 = new Prefix(_prefixConfig, "DEV");
            var p3 = new Prefix(_prefixConfig, "stageB");
            var p4 = new Prefix(_prefixConfig, "DEV", "iphone");
            var p5 = new Prefix(_prefixConfig, "ios", "iphone");

            var prefixes = new [] {p1, p2, p3, p4, p5};
            var max = prefixes.Max();
            var min = prefixes.Min();

            Assert.AreEqual(max, p1);
            Assert.AreEqual(min, p3);
            Assert.IsTrue(p2.CompareTo(p5) < 0);
        }
    }
}
