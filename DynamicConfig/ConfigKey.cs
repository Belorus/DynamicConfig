
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

        public override string ToString()
        {
            return  string.Format("[{0} : {1}]", Key, Prefix);
        }
    }
        
}