using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpYaml.Serialization;

namespace DynamicConfig
{
    internal class DynamicConfig : IDynamicConfig
    {
        internal const char PrefixSeparator = '-';

        private readonly List<Dictionary<object, object>> _configs;
        private readonly List<string> _prefixes;
        private HashSet<string> _prefixesSet; 
        private PrefixConfig _prefixConfig;

        private Dictionary<string, object> _config; 

        public DynamicConfig(params Stream[] configs)
        {
            _configs = new List<Dictionary<object, object>>(configs.Length);
            foreach (var config in configs)
            {
                using (var reader = new StreamReader(config, Encoding.UTF8, true, 1024, true))
                {
                    var formatter = new Serializer();
                    _configs.Add(formatter.Deserialize<Dictionary<object, object>>(reader));
                }    
            }
            
            _prefixes = new List<string>();
        }

        public void SetPrefixes(params string[] prefixes)
        {
            _prefixes.AddRange(prefixes);
        }

        public void InsertPrefix(int index, string prefix)
        {
            _prefixes.Insert(index, prefix);
        }

        public void Build()
        {
            _prefixConfig = new PrefixConfig(_prefixes.ToArray());
            _prefixesSet = new HashSet<string>(_prefixes);
            

            _config = ParseConfig();
        }

        public IEnumerable<string> AllKeys
        {
            get { return _config.Keys; }
        }

        public T Get<T>(string keyPath)
        {
            return (T) Convert.ChangeType(Get(keyPath), typeof (T));
        }

        public List<TItem> GetListOf<TItem>(string keyPath)
        {
            List<Object> list;
            if(!TryGet(keyPath, out list))
                return new List<TItem>();

            return new List<TItem>(list.Select(v => (TItem)Convert.ChangeType(v, typeof(TItem))));
        }

        public bool TryGet<T>(string keyPath, out T value)
        {
            object v;
            if (TryGet(keyPath, out v))
            {
                value = (T) Convert.ChangeType(v, typeof(T));
                return true;
            }

            value = default(T);
            return false;
        }

        private bool TryGet(string keyPath, out object value)
        {
            if (_config == null)
                throw new InvalidOperationException("Not initialized. Call method 'Build' first");

            return _config.TryGetValue(keyPath, out value);            
        }

        private object Get(string keyPath)
        {
            object value;
            if (!TryGet(keyPath, out value) || value == null)
            {
                throw new ArgumentException(string.Format("DynamicConfig keypath '{0}' is invalid. {1}Registered keys:{1}{2}", keyPath, Environment.NewLine, string.Join(Environment.NewLine, AllKeys)));
            }
            return value;
        }

        internal Dictionary<string, object> ParseConfig()
        {
            var parsedConfig = new Dictionary<string, object>();

            var mergetConfig = _configs.Where(c => c != null).SelectMany(_ => _);
                                    

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
                result = items.Aggregate((i1, i2) => i1.Key.Prefix.CompareTo(i2.Key.Prefix) > 0 ? i1 : i2).Value;

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
            
            if(!hasSimpleType)
            {
                result = dictionary
                    .GroupBy(
                        c => c.Key,
                        (k,v) => GroupRecursive(k, v, parsedConfig),
                        ConfigKey.KeyEqualityComparer.Comparer)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            return new KeyValuePair<ConfigKey, object>(key, result);
        }

        internal ConfigKey GetConfigKey(string key)
        {
            if (key[0] != PrefixSeparator)
            {
                return new ConfigKey(key, new Prefix(_prefixConfig));
            }
            key = key.TrimStart(PrefixSeparator);
            var prefixes = new List<string>();

            bool isValidPrefix;
            do
            {
                int separatorIndex = key.IndexOf(PrefixSeparator);
                if(separatorIndex < 0 || separatorIndex == key.Length - 1)
                    break;

                string part = key.Substring(0, separatorIndex);
                
                isValidPrefix = _prefixesSet.Contains(part);
                if (isValidPrefix)
                {
                    prefixes.Add(part);
                    key = key.Substring(separatorIndex + 1);
                }
                else
                {
                    key = key.TrimStart(PrefixSeparator);
                }

            } while (isValidPrefix);

            return new ConfigKey(key, new Prefix(_prefixConfig, prefixes.ToArray()));
        }
    }
}
