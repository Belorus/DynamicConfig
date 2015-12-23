using System;
using System.Collections.Generic;
using DynamicConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class GenerateConfigKeyTests : TestsBase
    {
        private readonly PrefixConfig _prefixConfig = new PrefixConfig(new List<string>{"a", "b", "c"});
        private readonly Version _version = new Version(1,0,0,0);

        [TestMethod]
        public void KeyGeneratorWithoutPrefixes()
        {
            // Aggange
            var config = new ConfigReader(new List<Dictionary<object, object>>(), _prefixConfig, _version);

            // Act
            var result = config.GetConfigKey(GenerateKey("key"));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("key", result.Key);
        }

        [TestMethod]
        public void KeyGeneratorWithPrefixes()
        {
            // Arrange
            var config = new ConfigReader(new List<Dictionary<object, object>>(), _prefixConfig, _version);

            // Act
            var result = config.GetConfigKey(GenerateKey("key", "a", "b"));


            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("key", result.Key);
            Assert.AreEqual(new Prefix(_prefixConfig, new List<string>{"a", "b"}), result.Prefix);
        }

        [TestMethod]
        public void KeyGeneratorWithInvalidPrefixes1()
        {
            // Arrange
            var config = new ConfigReader(new List<Dictionary<object, object>>(), _prefixConfig, _version);

            // Act
            var result = config.GetConfigKey(GenerateKey("key", "a", "b", "e"));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("e" + ConfigKey.PrefixSeparator + "key", result.Key);
            Assert.AreEqual(new Prefix(_prefixConfig, new List<string>{"a", "b"}), result.Prefix);
        }

        [TestMethod]
        public void KeyGeneratorWithInvalidPrefixes2()
        {
            // Arrange
            var config = new ConfigReader(new List<Dictionary<object, object>>(), _prefixConfig, _version);

            // Act
            var result = config.GetConfigKey(GenerateKey("key", "a", "e", "b"));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("e" + ConfigKey.PrefixSeparator + "b" + ConfigKey.PrefixSeparator + "key", result.Key);
            Assert.AreEqual(new Prefix(_prefixConfig, new List<string>{"a"}), result.Prefix);
        }

        [TestMethod]
        public void KeyGeneratorWithVersionRangePrefixes()
        {
            // Arrange
            var config = new ConfigReader(new List<Dictionary<object, object>>(), _prefixConfig, _version);

            // Act
            var result = config.GetConfigKey(GenerateKey("key", "a", "b", "(2.2.0-2.3.0)", "c"));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("key", result.Key);
            Assert.AreEqual(new Prefix(_prefixConfig, new List<string> { "a", "b", "c" }), result.Prefix);
        }
    }
}
