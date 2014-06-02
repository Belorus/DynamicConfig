using DynamicConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void BitsCountTest()
        {
            Assert.AreEqual(0, 0.BitsCount());
            Assert.AreEqual(1, 1.BitsCount());
            Assert.AreEqual(2, 10.BitsCount());
            Assert.AreEqual(3, 11.BitsCount());
            Assert.AreEqual(6, 1363.BitsCount());
        }
    }
}
