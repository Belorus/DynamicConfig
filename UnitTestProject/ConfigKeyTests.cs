using DynamicConfig;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestOf(typeof(ConfigKey.KeyEqualityComparer))]
    public class KeyEqualityComparer : TestsBase
    {
        private readonly PrefixBuilder _prefixBuilder = new PrefixBuilder(new []{"android", "ios", "w10"});
        private ConfigKeyBuilder _keyBuilder;

        [SetUp]
        public void Initialize()
        {
            _keyBuilder = new ConfigKeyBuilder(_prefixBuilder);
        }

        [TestCase("-android-ios-key", "-android-ios-key", true)]
        [TestCase("-android-ios-key", "-ios-android-key", true)]
        [TestCase("-android-ios-key", "-android-key", true)]
        [TestCase("-ios-key", "-android-ios-key", true)]
        [TestCase("-(2.0.0)-ios-key", "-(1.0.0)-ios-key", true)]
        [TestCase("-<0..10>-ios-key", "-<11..20>-ios-key", true)]
        [TestCase("-ios-key1", "-android-ios-key2", false)]
        [TestCase("key2", "key1", false)]
        public void Equals_TwoKeys_AreEqual(string key1String, string key2String, bool equals)
        {
            _keyBuilder.TryCreate(key1String, out var key1);
            _keyBuilder.TryCreate(key2String, out var key2);

            Assert.AreEqual(equals, ConfigKey.KeyEqualityComparer.Comparer.Equals(key1, key2));
            Assert.AreEqual(equals, ConfigKey.KeyEqualityComparer.Comparer.Equals(key2, key1));
        }
    }
}
