using System;
using System.Collections.Generic;
using DynamicConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ConfigKeyTests : TestsBase
    {
        private readonly PrefixBuilder _prefixBuilder = new PrefixBuilder(new List<string>{"a", "b", "c"});

        [TestMethod]
        public void KeyCompareTest()
        {
            VersionRange range;
            VersionRange.TryParse("1.0.0.0", out range);
            var key1 = new ConfigKey("key", _prefixBuilder.Create(new List<string> { "a", "b" }), range);
            var key2 = new ConfigKey("key", _prefixBuilder.Create(new List<string> { "a" }), range);
            var key3 = new ConfigKey("ttt", _prefixBuilder.Create(new List<string> { "a", "b" }), range);

            Assert.IsTrue (ConfigKey.KeyEqualityComparer.Comparer.Equals(key1, key2));
            Assert.IsFalse(ConfigKey.KeyEqualityComparer.Comparer.Equals(key2, key3));
        }

        [TestMethod]
        public void KeyWithVersionCompareTest()
        {
            Func<string, VersionRange> parse = (str) =>
            {
                VersionRange result;
                VersionRange.TryParse(str, out result);
                return result;
            };

            var key1 = new ConfigKey("key", _prefixBuilder.Create(new List<string>(0)), parse("1.0-2.0"));
            var key2 = new ConfigKey("key", _prefixBuilder.Create(new List<string>(0)), parse("1.0-2.0"));
            var key3 = new ConfigKey("key", _prefixBuilder.Create(new List<string>(0)), parse("0.9-2.0"));
            var key4 = new ConfigKey("key", _prefixBuilder.Create(new List<string>(0)), parse("1.1-2.0"));
            var key5 = new ConfigKey("key", _prefixBuilder.Create(new List<string>(0)), parse("1.0-3.0"));

            Assert.IsTrue(key1.CompareTo(key2) == 0);
            Assert.IsTrue(key1.CompareTo(key3) >  0);
            Assert.IsTrue(key1.CompareTo(key4) <  0);
            Assert.IsTrue(key1.CompareTo(key5) >  0);
            Assert.IsTrue(key3.CompareTo(key5) <  0);
        }

    }
}
