
namespace DynamicConfig
{
    internal partial class ConfigKey
    {
        public readonly string Key;
        public readonly Prefix Prefix;
        public readonly VersionRange VersionRange;

        public ConfigKey(string key, Prefix prefix) : this(key, prefix, VersionRange.Empty)
        {
        }

        public ConfigKey(string key, Prefix prefix, VersionRange versionRange)
        {
            Key = key;
            Prefix = prefix;
            VersionRange = versionRange;
        }

        public ConfigKey Merge(ConfigKey other)
        {
            return Merge(this, other);
        }

        public static ConfigKey Merge(ConfigKey first, ConfigKey second)
        {
            var newKey = string.Format("{0}:{1}", first.Key, second.Key);
            var newPrefix = Prefix.Merge(first.Prefix, second.Prefix);
            return new ConfigKey(newKey, newPrefix, second.VersionRange);
        }

        public override string ToString()
        {
            return  string.Format("[{0}{1} : {2}]", Key, VersionRange, Prefix);
        }
    }
        
}