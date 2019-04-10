namespace DynamicConfig
{
    internal partial class ConfigKey
    {
        public readonly string Key;
        public readonly Prefix Prefix;
        public readonly VersionRange VersionRange;
        public readonly Segment Segment;

        public ConfigKey(
            string key,
            Prefix prefix, 
            VersionRange versionRange,
            Segment segment)
        {
            Key = key;
            Prefix = prefix;
            VersionRange = versionRange;
            Segment = segment;
        }

        public ConfigKey Merge(ConfigKey other)
        {
            return Merge(this, other);
        }

        public static ConfigKey Merge(ConfigKey first, ConfigKey second)
        {
            var newKey = first.Key + ":" + second.Key;
            var newPrefix = Prefix.Merge(first.Prefix, second.Prefix);
            return new ConfigKey(newKey, newPrefix, second.VersionRange, second.Segment);
        }

        public override string ToString()
        {
            return $"[{Key}{VersionRange}{Segment} : {Prefix}]";
        }
    }
}