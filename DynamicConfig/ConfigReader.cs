using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    internal class ConfigReader
    {
        private readonly List<Dictionary<object, object>> _configs;
        private readonly ConfigKeyBuilder _keyBuilder;

        private readonly Version _appVersion;
        private readonly IComparer<Version> _versionComparer;

        private class ConfigKeyItem
        {
            public ConfigKey Key;
        }

        private class ConfigKeyValue : ConfigKeyItem
        {
            public object Value;
        }

        public ConfigReader(List<Dictionary<object, object>> configs, IPrefixBuilder prefixBuilder, Version appVersion, IComparer<Version> versionComparer)
        {
            _configs = configs;
            _appVersion = appVersion;
            _versionComparer = versionComparer;
            _keyBuilder = new ConfigKeyBuilder(prefixBuilder);
        }

        public Dictionary<string, object> ParseConfig()
        {
            var parsedConfig = new Dictionary<ConfigKey, ConfigKeyItem>(ConfigKey.KeyEqualityComparer.Comparer);

            int itemsCount = 0;
            foreach (var config in _configs)
            {
                itemsCount += ParseRecursive(config, parsedConfig, null);
            }

            var result = new Dictionary<string, object>(itemsCount);
            foreach (var kv in parsedConfig)
            {
                var configKeyValue = kv.Value as ConfigKeyValue;
                if (configKeyValue != null)
                {
                    result.Add(kv.Key.Key, configKeyValue.Value);
                }
            }

            return result;
        }

        private int ParseRecursive(Dictionary<object, object> rawConfig, Dictionary<ConfigKey, ConfigKeyItem> config, ConfigKey parentKey)
        {
            int itemsCount = 0;
            foreach (var pair in rawConfig)
            {
                ConfigKey key;
                if (_keyBuilder.TryCreate(pair.Key.ToString(), out key))
                {
                    key = parentKey != null ? parentKey.Merge(key) : key;
                    object value = pair.Value;

                    if (!key.VersionRange.InRange(_appVersion, _versionComparer))
                    {
                        continue;
                    }

                    ConfigKeyItem keyItem;
                    bool keyExists = config.TryGetValue(key, out keyItem);
                    ConfigKeyValue valueItem = keyItem as ConfigKeyValue;

                    var dict = value as Dictionary<object, object>;
                    if (dict != null)
                    {
                        if (valueItem != null)
                        {
                            throw new DynamicConfigException(string.Format("Key '{0}' have different value types", key.Key));
                        }

                        config[key] = new ConfigKeyItem
                        {
                            Key = key
                        };
                        itemsCount += ParseRecursive(dict, config, key);
                    }
                    else
                    {
                        if (keyExists)
                        {
                            if (valueItem == null)
                            {
                                throw new DynamicConfigException(string.Format("Key '{0}' have different value types", key.Key));
                            }

                            if (key.CompareTo(keyItem.Key) > 0)
                            {
                                valueItem.Key = key;
                                valueItem.Value = value;
                            }
                        }
                        else
                        {
                            itemsCount++;
                            config[key] = new ConfigKeyValue
                            {
                                Key = key,
                                Value = value
                            };
                        }
                    }
                }
            }

            return itemsCount;
        }
    }
}
