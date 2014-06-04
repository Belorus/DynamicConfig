using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ConfigTests : TestsBase
    {
        private readonly string[] _prefixes = {"a", "b"};

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyConfigTest1()
        {
            var config = CreateConfig(new string[0], _prefixes);

            // Act
            config.Get<string>("invalidKey");

            // Assert - Expects exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyConfigTest2()
        {
            var config = CreateConfig(new[] {TestData.EmptyData}, _prefixes);

            // Act
            config.Get<string>("invalidKey");

            // Assert - Expects exception
        }

        [TestMethod]
        public void GroupTest1()
        {
            // Arrange
            var config = (DynamicConfig.DynamicConfig)CreateConfig(TestData.Data2, _prefixes);

            // Act
            var groups = config.ParseConfig();

            // Assert
            Assert.AreEqual(7, groups.Count);
        }

        [TestMethod]
        public void TwoConfigMergeTest()
        {
            // Arrange
            var config = (DynamicConfig.DynamicConfig)CreateConfig(new[] { TestData.Data2, TestData.Data3 }, _prefixes);

            // Act
            var groups = config.ParseConfig();

            // Assert
            Assert.AreEqual(8, groups.Count);
        }

        [TestMethod]
        public void GetDataFromConfigTest()
        {
            // Arrange 
            var config = CreateConfig(new[] { TestData.Data2, TestData.Data3 }, _prefixes);

            // Act
            var result1 = config.Get<string>("a:a:c:e");
            var result2 = config.Get<string>("c:k1");
            var result3 = config.Get<string>("d:k1");

            // Assert
            Assert.AreEqual("-a-a-a-c-e", result1);
            Assert.AreEqual("-a-c-k1", result2);
            Assert.AreEqual("d-k1", result3);
        }

        [TestMethod]
        public void ConfigWithoutRootTestTest()
        {
            // Arrange 
            var config = CreateConfig(new[] { TestData.DataWithoutRoot }, _prefixes);

            // Act
            var result = config.Get<string>("key1");

            // Assert
            Assert.AreEqual("key1", result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetInvelidDataFromConfigTest()
        {
            // Arrange
            var config = CreateConfig(new[] { TestData.Data2, TestData.Data3 }, _prefixes);

            // Act
            config.Get<string>("invalidKey");

            // Assert - Expects exception
        }
    }
}
