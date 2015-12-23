using System.Collections.Generic;
using DynamicConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ConfigKeyTests : TestsBase
    {
        private readonly PrefixConfig _prefixConfig = new PrefixConfig(new List<string>{"a", "b", "c"});

        [TestMethod]
        public void StrictCompareTest()
        {
            var key1 = new ConfigKey("key", new Prefix(_prefixConfig, new List<string>{"a", "b"}));
            var key2 = new ConfigKey("key", new Prefix(_prefixConfig, new List<string>{"a"}));
            var key3 = new ConfigKey("key", new Prefix(_prefixConfig, new List<string>{"a", "b"}));
            var key4 = new ConfigKey("ttt", new Prefix(_prefixConfig, new List<string>{"a", "b"}));

            Assert.IsTrue (ConfigKey.StrictEqualityComparer.Comparer.Equals(key1, key3));
            Assert.IsFalse(ConfigKey.StrictEqualityComparer.Comparer.Equals(key1, key2));
            Assert.IsFalse(ConfigKey.StrictEqualityComparer.Comparer.Equals(key1, key4));

        }

        [TestMethod]
        public void KeyCompareTest()
        {
            var key1 = new ConfigKey("key", new Prefix(_prefixConfig, new List<string>{"a", "b"}));
            var key2 = new ConfigKey("key", new Prefix(_prefixConfig, new List<string>{"a"}));
            var key3 = new ConfigKey("ttt", new Prefix(_prefixConfig, new List<string>{"a", "b"}));

            Assert.IsTrue (ConfigKey.KeyEqualityComparer.Comparer.Equals(key1, key2));
            Assert.IsFalse(ConfigKey.KeyEqualityComparer.Comparer.Equals(key2, key3));

        }

    }
}
