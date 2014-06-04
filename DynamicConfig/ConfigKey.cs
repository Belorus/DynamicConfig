
namespace DynamicConfig
{
    internal partial class ConfigKey
    {
        public string Key { get; private set; }
        public Prefix Prefix { get; private set; }

        public ConfigKey(string key, Prefix prefix)
        {
            Key = key;
            Prefix = prefix;
        }

        public ConfigKey Merge(ConfigKey other)
        {
            return Merge(this, other);
        }

        public static ConfigKey Merge(ConfigKey first, ConfigKey second)
        {
            var newKey = string.Format("{0}:{1}", first.Key, second.Key);
            var newPrefix = Prefix.Merge(first.Prefix, second.Prefix);
            return new ConfigKey(newKey, newPrefix);
        }

        public override string ToString()
        {
            return  string.Format("[{0} : {1}]", Key, Prefix);
        }
    }
        
}