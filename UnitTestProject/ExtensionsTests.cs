using DynamicConfig;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestOf(typeof(TypeExtensions))]
    public class ExtensionsTests
    {
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(2, 10)]
        [TestCase(3, 11)]
        [TestCase(6, 1363)]
        public void BitsCountTest(int expectedBitsCount, int number)
        {
            Assert.AreEqual(expectedBitsCount, number.BitsCount());
        }
    }
}
