using System.Collections.Generic;
using System.Linq;
using DynamicConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class PrefixTests
    {
        private readonly PrefixBuilder _prefixBuilder = new PrefixBuilder(new List<string>{"DEV", "ios", "iphone", "stageB"});
                                                                       
        [TestMethod]
        public void EqualityTest()
        {
            var p1 = _prefixBuilder.Create(new List<string> { "DEV", "iphone" });
            var p2 = _prefixBuilder.Create(new List<string> { "DEV" });
            var p3 = _prefixBuilder.Create(new List<string> { "stageB" });
            var p4 = _prefixBuilder.Create(new List<string> { "DEV", "iphone" });

            Assert.IsTrue(p1.CompareTo(p2) > 0);
            Assert.IsTrue(p1.CompareTo(p3) > 0);
            Assert.IsTrue(p2.CompareTo(p3) > 0);
            Assert.AreNotEqual(p1, p2);
            Assert.AreEqual(p1, p4);
        }

        [TestMethod]
        public void CompareTest()
        {
            var p1 = _prefixBuilder.Create(new List<string> { "DEV", "iphone" });
            var p2 = _prefixBuilder.Create(new List<string> { "DEV" });
            var p3 = _prefixBuilder.Create(new List<string> { "stageB" });
            var p4 = _prefixBuilder.Create(new List<string> { "DEV", "iphone" });
            var p5 = _prefixBuilder.Create(new List<string> { "ios", "iphone" });

            var prefixes = new [] {p1, p2, p3, p4, p5};
            var max = prefixes.Max();
            var min = prefixes.Min();

            Assert.AreEqual(max, p1);
            Assert.AreEqual(min, p3);
            Assert.IsTrue(p2.CompareTo(p5) < 0);
        }
    }
}
