using DynamicConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class GenerateConfigKeyTests : TestsBase
    {
        private readonly string[] _prefixes = {"a", "b", "c"};
        private readonly PrefixConfig _prefixConfig = new PrefixConfig("a", "b", "c");

        [TestMethod]
        public void KeyGeneratorWithoutPrefixes()
        {
            // Aggange
            var config = CreateConfig(TestData.SimpleData, _prefixes);

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
            var config = CreateConfig(TestData.SimpleData, _prefixes);

            // Act
            var result = config.GetConfigKey(GenerateKey("key", "a", "b"));


            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("key", result.Key);
            Assert.AreEqual(new Prefix(_prefixConfig, "a", "b"), result.Prefix);
        }

        [TestMethod]
        public void KeyGeneratorWithInvalidPrefixes1()
        {
            // Arrange
            var config = CreateConfig(TestData.SimpleData, _prefixes);

            // Act
            var result = config.GetConfigKey(GenerateKey("key", "a", "b", "e"));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("e" + DynamicConfig.DynamicConfig.PrefixSeparator + "key", result.Key);
            Assert.AreEqual(new Prefix(_prefixConfig, "a", "b"), result.Prefix);
        }

        [TestMethod]
        public void KeyGeneratorWithInvalidPrefixes2()
        {
            // Arrange
            var config = CreateConfig(TestData.SimpleData, _prefixes);

            // Act
            var result = config.GetConfigKey(GenerateKey("key", "a", "e", "b"));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("e" + DynamicConfig.DynamicConfig.PrefixSeparator + "b" + DynamicConfig.DynamicConfig.PrefixSeparator + "key", result.Key);
            Assert.AreEqual(new Prefix(_prefixConfig, "a"), result.Prefix);
        }
    }
}
