using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicConfig
{
    internal class ConfigReader
    {
        private readonly List<Dictionary<object, object>> _configs;
        private readonly PrefixConfig _prefixConfig;
        private readonly Version _version;

        public ConfigReader(List<Dictionary<object, object>> configs, PrefixConfig prefixConfig, Version version)
        {
            _configs = configs;
            _prefixConfig = prefixConfig;
            _version = version;
        }

        public Dictionary<string, object> ParseConfig()
        {
            var parsedConfig = new Dictionary<string, object>();

            var mergetConfig = _configs.SelectMany(_ => _);
            mergetConfig
                .Select(c => new KeyValuePair<ConfigKey, object>(GetConfigKey(c.Key.ToString()), c.Value))
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
                        var objectKey = GetConfigKey(o.Key.ToString());
                        var newKey = ConfigKey.Merge(pair.Key, objectKey);
                        dictionary.Add(newKey, o.Value);
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
                result = items.Aggregate(SelectKey).Value;

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

        private KeyValuePair<ConfigKey, object> SelectKey(KeyValuePair<ConfigKey, object> firstKey, KeyValuePair<ConfigKey, object> secondKey)
        {
            bool isFirstKeyVerionSupport = firstKey.Key.VersionRange.InRange(_version);
            bool isSecondKeyVerionSupport = secondKey.Key.VersionRange.InRange(_version);

            if ((isFirstKeyVerionSupport && isSecondKeyVerionSupport) || (!isFirstKeyVerionSupport && !isSecondKeyVerionSupport))
                return firstKey.Key.Prefix.CompareTo(secondKey.Key.Prefix) > 0 ? firstKey : secondKey;

            if (isSecondKeyVerionSupport)
                return secondKey;

            return firstKey;
        }

        internal ConfigKey GetConfigKey(string key)
        {
            return ConfigKey.Parse(key, _prefixConfig);
        }
    }
}
