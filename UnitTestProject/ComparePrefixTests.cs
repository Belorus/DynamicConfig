using System;
using DynamicConfig;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestOf(typeof(PrefixBuilder))]
    public class ComparePrefixTests
    {
        private readonly PrefixBuilder _prefixBuilder = new PrefixBuilder(new[] { "DEV", "ios", "iphone", "stageB" });

        [TestCase(new[] { "DEV" }, new[] { "DEV" })]
        [TestCase(new[] { "DEV", "ios" }, new[] { "DEV", "ios" })]
        [TestCase(new[] { "DEV", "ios" }, new[] { "ios", "DEV" })]
        [TestCase(new[] { "ios", "DEV" }, new[] { "DEV", "ios" })]
        [TestCase(new[] { "ios", "iphone", "DEV" }, new[] { "DEV", "ios", "iphone" })]
        public void CompareTo_Prefixes_LeftEqualToRight(string[] left, string[] right)
        {
            var leftPrefix = _prefixBuilder.Create(left);
            var rightPrefix = _prefixBuilder.Create(right);

            Assert.AreEqual(leftPrefix, rightPrefix);
            Assert.IsTrue(leftPrefix.CompareTo(rightPrefix) == 0);
            Assert.IsTrue(rightPrefix.CompareTo(leftPrefix) == 0);
        }

        [TestCase(new[] { "DEV" }, new[] { "ios" })]
        [TestCase(new[] { "DEV", "ios" }, new[] { "DEV" })]
        [TestCase(new[] { "DEV", "ios" }, new[] { "DEV", "iphone" })]
        [TestCase(new[] { "DEV", "ios", "stageB" }, new[] { "DEV", "iphone", "stageB" })]
        [TestCase(new[] { "DEV", "iphone", "stageB" }, new[] { "DEV", "ios" })]
        public void CompareTo_Prefixes_LeftMoreThanRight(string[] left, string[] right)
        {
            var leftPrefix = _prefixBuilder.Create(left);
            var rightPrefix = _prefixBuilder.Create(right);

            Assert.AreNotEqual(leftPrefix, rightPrefix);
            Assert.IsTrue(leftPrefix.CompareTo(rightPrefix) > 0);
            Assert.IsTrue(rightPrefix.CompareTo(leftPrefix) < 0);
        }

        [Test]
        public void Create_InvalidPrefix_Exception()
        {
            const string invalidPrefix = "win32";

            Assert.IsFalse(_prefixBuilder.Contains(invalidPrefix));
            Assert.Throws<NotSupportedException>(() => _prefixBuilder.Create(new[] { invalidPrefix }));
        }
    }
}
