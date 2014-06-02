using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class ConfigTests : TestsBase
    {
        private readonly string[] _prefixes = {"a", "b"};

        [TestMethod]
        public void GroupTest1()
        {
            // Arrange
            var config = CreateConfig(TestData.Data2, _prefixes);

            // Act
            var groups = config.ParseConfig();

            // Assert
            Assert.AreEqual(7, groups.Count);
        }

        [TestMethod]
        public void TwoConfigMergeTest()
        {
            // Arrange
            var config = CreateConfig(new []{TestData.Data2, TestData.Data3}, _prefixes);

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
            var result = config.Get<string>("e");

            // Assert
            Assert.AreEqual("-a-a-a-c-e", result);
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

            Assert.AreEqual("-a-a-a-c-e", config.Get<string>("e"));

            // Assert - Expects exception
        }
    }
}
