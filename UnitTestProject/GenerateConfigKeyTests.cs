using System.Collections.Generic;
using DynamicConfig;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestOf(typeof(ConfigKeyBuilder))]
    public class ConfigKeyBuilderTests : TestsBase
    {
        private readonly PrefixBuilder _prefixBuilder = new PrefixBuilder(new[] {"android", "ios"});
        private ConfigKeyBuilder _keyBuilder;

        [SetUp]
        public void Initialize()
        {
            _keyBuilder = new ConfigKeyBuilder(_prefixBuilder);
        }

        [TestCase("key")]
        [TestCase("key", new[] { "android" })]
        [TestCase("key", new[] { "android", "ios" })]
        [TestCase("key", new[] { "android", "ios" }, "1.0.0-2.0.0")]
        [TestCase("key", new[] { "android", "ios" }, "1.0.0-")]
        [TestCase("key", new[] { "android", "ios" }, "-1.0.0")]
        [TestCase("key", new[] { "android", "ios" }, "1.0.0")]
        [TestCase("key", new[] { "android", "ios" }, "1.0.0-2.0.0", "0..10")]
        [TestCase("key", new[] { "android", "ios" }, "1.0.0-2.0.0", "*")]
        [TestCase("key", new[] { "android", "ios" }, null, "*")]
        [TestCase("key", new[] { "android", "ios" }, null, "0..10")]
        [TestCase("key", null, "1.0.0-2.0.0", "*")]
        [TestCase("key", null, "1.0.0-2.0.0")]
        [TestCase("key", null, null, "*")]
        public void TryCreate_KeyString_Success(string keyString, string[] prefixes = null, string versionRange = null, string segmentation = null)
        {
            var generatedKey = GenerateKey(keyString, ToPrefixes(prefixes, versionRange, segmentation));

            var createResult = _keyBuilder.TryCreate(generatedKey, out var key);

            Assert.IsTrue(createResult);
            Assert.IsNotNull(key);
            Assert.AreEqual(keyString, key.Key);
            Assert.AreEqual(_prefixBuilder.Create(prefixes ?? new string[0]), key.Prefix);

            VersionRange.TryParse(versionRange ?? string.Empty, out var expectedVersionRange);
            expectedVersionRange = expectedVersionRange ?? VersionRange.Empty;
            Assert.IsTrue(expectedVersionRange.CompareTo(key.VersionRange) == 0);

            Segment.TryParse(segmentation ?? string.Empty, out var expectedSegmentation);
            expectedSegmentation = expectedSegmentation ?? Segment.Default;
            Assert.IsTrue(expectedSegmentation.CompareTo(key.Segment) == 0);
        }

        [Test]
        public void TryCreate_KeyWithUnregisteredPrefixes_CantCreate()
        {
            var keyString = GenerateKey("key", "android", "ios", "win32");

            var result = _keyBuilder.TryCreate(keyString, out var key);

            Assert.IsFalse(result);
            Assert.IsNull(key);
        }

        [TestCase("key", null, "1.0.0-2.0.0.fail", "*")]
        [TestCase("key", null, "1.0.0-2.0.0", "fail")]
        [TestCase("key", null, "-1.0.0-")]
        [TestCase("key", null, null, "fail.fail")]
        public void TryCreate_InvalidData_Exception(string keyString, string[] prefixes = null, string versionRange = null, string segmentation = null)
        {
            var generatedKey = GenerateKey(keyString, ToPrefixes(prefixes, versionRange, segmentation));

            Assert.Throws<DynamicConfigException>(() => _keyBuilder.TryCreate(generatedKey, out var _));
        }

        private static string[] ToPrefixes(string[] prefixes, string versionRange, string segmentation)
        {
            var result = new List<string>();
            if (versionRange != null)
            {
                result.Add($"({versionRange})");
            }

            if (segmentation != null)
            {
                result.Add($"<{segmentation}>");
            }

            if (prefixes != null)
            {
                result.AddRange(prefixes);
            }

            return result.ToArray();
        }
    }
}
