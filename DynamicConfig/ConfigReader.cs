using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicConfig
{
    internal class ConfigReader
    {
        private readonly List<Dictionary<object, object>> _configs;
        private readonly ConfigKeyBuilder _keyBuilder;

        private class ConfigKeyValue
        {
            public ConfigKey Key;
            public object Value;
        }

        public ConfigReader(List<Dictionary<object, object>> configs, PrefixBuilder prefixBuilder, Version version)
        {
            _configs = configs;
            _keyBuilder = new ConfigKeyBuilder(prefixBuilder, version);
        }

        public Dictionary<string, object> ParseConfig()
        {
            var parsedConfig = new Dictionary<ConfigKey, ConfigKeyValue>(ConfigKey.KeyEqualityComparer.Comparer);

            foreach (var config in _configs)
            {
                ParseRecursive(config, parsedConfig, null);
            }

            return parsedConfig.ToDictionary(kv => kv.Key.Key, kv => kv.Value.Value);
        }

        private void ParseRecursive(Dictionary<object, object> rawConfig, Dictionary<ConfigKey, ConfigKeyValue> config, ConfigKey parentKey)
        {
            foreach (var pair in rawConfig)
            {
                ConfigKey key;
                if (_keyBuilder.TryCreate(pair.Key.ToString(), out key))
                {
                    key = parentKey != null ? parentKey.Merge(key) : key;
                    object value = pair.Value;

                    var dict = value as Dictionary<object, object>;
                    if (dict != null)
                    {
                        ParseRecursive(dict, config, key);
                    }
                    else
                    {
                        ConfigKeyValue kv;
                        if (config.TryGetValue(key, out kv))
                        {
                            if (key.CompareTo(kv.Key) > 0)
                            {
                                kv.Key = key;
                                kv.Value = value;
                            }
                        }
                        else
                        {
                            config[key] = new ConfigKeyValue
                            {
                                Key = key,
                                Value = value
                            };
                        }
                    }
                }
            }
        }
    }
}
