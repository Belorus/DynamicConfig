using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicConfig
{
    internal class ConfigReader
    {
        private readonly List<Dictionary<object, object>> _configs;
        private readonly ConfigKeyBuilder _keyBuilder;

        public ConfigReader(List<Dictionary<object, object>> configs, PrefixBuilder prefixBuilder, Version version)
        {
            _configs = configs;
            _keyBuilder = new ConfigKeyBuilder(prefixBuilder, version);
        }

        public Dictionary<string, object> ParseConfig()
        {
            var parsedConfig = new Dictionary<string, object>();

            List<KeyValuePair<ConfigKey, object>> mergedConfigs = new List<KeyValuePair<ConfigKey, object>>();
            foreach (var config in _configs)
            {
                foreach (var kv in config)
                {
                    ConfigKey configKey;
                    if (_keyBuilder.TryCreate(kv.Key.ToString(), out configKey))
                    {
                        mergedConfigs.Add(new KeyValuePair<ConfigKey, object>(configKey, kv.Value));
                    }
                }
            }

            mergedConfigs
            .GroupBy(
                c => c.Key,
                (k, v) => GroupRecursive(k, v, parsedConfig),
                ConfigKey.KeyEqualityComparer.Comparer).ToList();

            return parsedConfig;
        }

        private KeyValuePair<ConfigKey, object> GroupRecursive(ConfigKey key, IEnumerable<KeyValuePair<ConfigKey, object>> values, IDictionary<string, object> parsedConfig)
        {
            var items = values.ToList();
            var dictionary = new Dictionary<ConfigKey, object>(ConfigKey.StrictEqualityComparer.Comparer);

            bool hasSimpleType = false;

            foreach (var pair in items)
            {
                var value = pair.Value as IDictionary<object, object>;
                if (value != null)
                {
                    foreach (var o in value)
                    {
                        ConfigKey objectKey;
                        if (_keyBuilder.TryCreate(o.Key.ToString(), out objectKey))
                        {
                            var newKey = ConfigKey.Merge(pair.Key, objectKey);
                            dictionary.Add(newKey, o.Value);
                        }
                    }
                }
                else
                {
                    hasSimpleType = true;
                }
            }

            object result = null;
            if (hasSimpleType)
            {
                result = items.Aggregate((i1, i2) => i1.Key.CompareTo(i2.Key) > 0 ? i1 : i2).Value;

                if (result is IDictionary<object, object>)
                {
                    //TODO: maybe better throw exception?
                    hasSimpleType = false;
                }
                else
                {
                    parsedConfig.Add(key.Key, result);
                }
            }

            if (!hasSimpleType)
            {
                result = dictionary
                    .GroupBy(
                        c => c.Key,
                        (k, v) => GroupRecursive(k, v, parsedConfig),
                        ConfigKey.KeyEqualityComparer.Comparer)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            return new KeyValuePair<ConfigKey, object>(key, result);
        }
    }
}
