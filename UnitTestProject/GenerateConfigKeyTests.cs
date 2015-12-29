using System;
using System.Collections.Generic;
using DynamicConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class GenerateConfigKeyTests : TestsBase
    {
        private readonly PrefixBuilder _prefixBuilder = new PrefixBuilder(new List<string> {"a", "b", "c"});
        private ConfigKeyBuilder _keyBuilder;

        [TestInitialize]
        public void Initialize()
        {
            _keyBuilder = new ConfigKeyBuilder(_prefixBuilder, new Version(1,0));
        }

        [TestMethod]
        public void KeyGeneratorWithoutPrefixes()
        {
            // Aggange
            string keyString = GenerateKey("key");

            // Act
            ConfigKey key;
            var result = _keyBuilder.TryCreate(keyString, out key);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(key);
            Assert.AreEqual("key", key.Key);
        }

        [TestMethod]
        public void KeyGeneratorWithPrefixes()
        {
            // Arrange
            string keyString = GenerateKey("key", "a", "b");

            // Act
            ConfigKey key;
            var result = _keyBuilder.TryCreate(keyString, out key);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(key);
            Assert.AreEqual("key", key.Key);
            Assert.AreEqual(_prefixBuilder.Create(new List<string> { "a", "b" }), key.Prefix);
        }

        [TestMethod]
        public void KeyGeneratorWithInvalidPrefixes1()
        {
            // Arrange
            string keyString = GenerateKey("key", "a", "b", "e");

            // Act
            ConfigKey key;
            var result = _keyBuilder.TryCreate(keyString, out key);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(key);
        }

        [TestMethod]
        public void KeyGeneratorWithInvalidPrefixes2()
        {
            // Arrange
            string keyString = GenerateKey("key", "a", "e", "b");

            // Act
            ConfigKey key;
            var result = _keyBuilder.TryCreate(keyString, out key);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(key);
        }

        [TestMethod]
        public void KeyGeneratorWithVersionRangePrefixes()
        {
            // Arrange
            string keyString = GenerateKey("key", "a", "b", "(0.2.0-2.3.0)", "c");

            // Act
            ConfigKey key;
            var result = _keyBuilder.TryCreate(keyString, out key);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(key);
            Assert.AreEqual("key", key.Key);
            Assert.AreEqual(_prefixBuilder.Create(new List<string> { "a", "b", "c" }), key.Prefix);
        }

        [TestMethod]
        public void KeyGeneratorWithInvalidVersionRangePrefixes()
        {
            // Arrange
            string keyString = GenerateKey("key", "a", "b", "(0.2.0-2.3.fail)", "c");

            // Act
            ConfigKey key;
            var result = _keyBuilder.TryCreate(keyString, out key);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(key);
        }
    }
}
