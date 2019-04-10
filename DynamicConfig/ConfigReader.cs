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
        private readonly ISegmentChecker _segmentChecker;

        private class ConfigKeyItem
        {
            public ConfigKey Key;
        }

        private class ConfigKeyValue : ConfigKeyItem
        {
            public object Value;
        }

        public ConfigReader(List<Dictionary<object, object>> configs,
            IPrefixBuilder prefixBuilder,
            Version appVersion,
            IComparer<Version> versionComparer,
            ISegmentChecker segmentChecker)
        {
            _configs = configs;
            _appVersion = appVersion;
            _versionComparer = versionComparer;
            _segmentChecker = segmentChecker;
            _keyBuilder = new ConfigKeyBuilder(prefixBuilder);
        }

        public Dictionary<string, object> ParseConfig()
        {
            var parsedConfig = new Dictionary<ConfigKey, ConfigKeyItem>(ConfigKey.KeyEqualityComparer.Comparer);

            var itemsCount = 0;
            foreach (var config in _configs)
            {
                itemsCount += ParseRecursive(config, parsedConfig, null);
            }

            var result = new Dictionary<string, object>(itemsCount);
            foreach (var kv in parsedConfig)
            {
                if (kv.Value is ConfigKeyValue configKeyValue)
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
                if (_keyBuilder.TryCreate(pair.Key.ToString(), out var key))
                {
                    key = parentKey != null ? parentKey.Merge(key) : key;
                    var value = pair.Value;

                    if (!key.VersionRange.InRange(_appVersion, _versionComparer))
                    {
                        continue;
                    }

                    if (!_segmentChecker.Check(key.Segment))
                    {
                        continue;
                    }

                    var keyExists = config.TryGetValue(key, out var keyItem);
                    var valueItem = keyItem as ConfigKeyValue;

                    if (value is Dictionary<object, object> dict)
                    {
                        if (valueItem != null)
                        {
                            throw new DynamicConfigException($"Key '{key.Key}' have different value types");
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
                                throw new DynamicConfigException($"Key '{key.Key}' have different value types");
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
