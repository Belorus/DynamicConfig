using System;
using System.Linq;
using DynamicConfig;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestOf(typeof(DynamicConfig.DynamicConfig))]
    public class ConfigTests : TestsBase
    {
        private static readonly string[] Prefixes = {"IOS"};
        private static readonly string[] EmptyPrefixes = new string[0];

        [TestCase(arg: new string[0])]
        [TestCase(arg: new []{""})]
        [TestCase(arg: new []{TestData.Config})]
        public void CreateConfig_GetValueByInvalidKey_Exception(string[] configs)
        {
            var config = CreateConfig(configs, Prefixes);
            Assert.Throws<ArgumentException>(() => config.Get<string>("invalidKey"));
        }

        public void GetValue_ValidKeyWithoutPrefixes_CorrectValue()
        {
            var config = CreateConfig(TestData.Config, EmptyPrefixes);

            var timeout = config.Get<int>("http:timeout");

            Assert.That(timeout, Is.EqualTo(2000));
        }

        [Test]
        public void GetValue_ValidKeyWithPrefixes_CorrectValue()
        {
            var config = CreateConfig(TestData.Config, Prefixes);

            var timeout = config.Get<int>("http:timeout");

            Assert.That(timeout, Is.EqualTo(1000));
        }

        [Test]
        public void AllKeys_CheckCount_CorrectKeysCount()
        {
            var config = CreateConfig(TestData.Config, Prefixes);

            var groups = config.AllKeys;

            Assert.AreEqual(5, groups.Count());
        }

        [Test]
        public void TwoConfigMerge_CheckKeysCount_CorrectKeysCount()
        {
            var config = CreateConfig(new[] { TestData.Config, TestData.AddNewFields }, Prefixes);

            var groups = config.AllKeys;

            Assert.That(groups.Count(), Is.EqualTo(6));
        }

        [Test]
        public void TwoConfigMerge_GetValueFromBothConfigs_Correctvalues()
        {
            var config = CreateConfig(new[] { TestData.Config, TestData.AddNewFields }, EmptyPrefixes);

            var valueFromFirstConfig = config.Get<int>("http:timeout");
            var valueFromSecondConfig = config.Get<int>("http:throttle_interval");

            Assert.That(valueFromFirstConfig, Is.EqualTo(2000));
            Assert.That(valueFromSecondConfig, Is.EqualTo(1000));
        }

        [Test]
        public void MultipleBuild_GetValue_DifferentValues()
        {
            var config = CreateConfig(new[] { TestData.Config }, EmptyPrefixes);

            var beforeSecondBuild = config.Get<int>("http:timeout");

            config.Build(new DynamicConfigOptions{Prefixes = Prefixes});

            var afterSecondBuild = config.Get<int>("http:timeout");

            Assert.That(beforeSecondBuild, Is.EqualTo(2000));
            Assert.That(afterSecondBuild, Is.EqualTo(1000));
        }

        [Test]
        public void BuildCOnfig_DifferentValueType_Exception()
        {
            Assert.Throws<DynamicConfigException>(() => CreateConfig(new[] { TestData.Config, TestData.OverrideCompressionWithInvalidStructure }, Prefixes));
        }

        [Test]
        public void Get_Enum_CorrectValue()
        {
            var config = CreateConfig(TestData.Config, EmptyPrefixes);

            var enumFromString = config.Get<TestData.CompressionMode>("http:compression:mode");
            var enumFromInt = config.Get<TestData.CompressionMode>("http:compression:int_mode");

            Assert.That(enumFromString, Is.EqualTo(TestData.CompressionMode.GZip));
            Assert.That(enumFromInt, Is.EqualTo(TestData.CompressionMode.Deflate));
        }

        [TestCase("1.0.0.0", 3000)]
        [TestCase("3.5.0.0", 4000)]
        [TestCase("5.0.0.0", 2000)]
        public void GetValue_SetAppVersion_CorrectValue(string version, int result)
        {
            var config = CreateConfig(new[] { TestData.Config }, EmptyPrefixes);
            config.Build(new DynamicConfigOptions { Prefixes = EmptyPrefixes, AppVersion = Version.Parse(version) });

            var value = config.Get<int>("http:timeout");

            Assert.That(value, Is.EqualTo(result));
        }

        [TestCase(5, 10)]
        [TestCase(30, 40)]
        [TestCase(90, 50)]
        public void GetValue_SetSegment_CorrectValue(int seed, int result)
        {
            var config = CreateConfig(new[] { TestData.Config }, EmptyPrefixes);
            config.Build(new DynamicConfigOptions { Prefixes = EmptyPrefixes, SegmentChecker = new SegmentChecker(seed)});

            var value = config.Get<int>("http:retry_count");

            Assert.That(value, Is.EqualTo(result));
        }
    }
}
