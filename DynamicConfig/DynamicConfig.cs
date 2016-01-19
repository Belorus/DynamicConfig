using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DynamicConfig.SharpYaml;
using SharpYaml.Serialization;

namespace DynamicConfig
{
    internal class DynamicConfig : IDynamicConfig
    {
        private readonly List<Dictionary<object, object>> _configs;
        private readonly List<string> _prefixes;
        private PrefixBuilder _prefixBuilder;

        private Dictionary<string, object> _config;
        private Version _applicationVersion;

        public DynamicConfig(params Stream[] configs)
        {
            _configs = new List<Dictionary<object, object>>(configs.Length);
            var formatter = new Serializer(new SerializerSettings {Attributes = new EmptyAttributeRegistry(), EmitAlias = false});
            foreach (var config in configs)
            {
                using (var reader = new StreamReader(config, Encoding.UTF8, true, 1024, true))
                {
                    var dict = formatter.Deserialize<Dictionary<object, object>>(reader);
                    if (dict != null)
                        _configs.Add(dict);
                }
            }

            _prefixes = new List<string>();
        }

        public void SetApplicationVersion(Version version)
        {
            _applicationVersion = version;
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
            _prefixBuilder = new PrefixBuilder(_prefixes);

            var configReader = new ConfigReader(_configs, _prefixBuilder, _applicationVersion);

            _config = configReader.ParseConfig();
        }

        public IEnumerable<string> AllKeys
        {
            get { return _config.Keys; }
        }

        public T Get<T>(string keyPath)
        {
            return (T) Convert.ChangeType(Get(keyPath), typeof (T));
        }

        public TItem[] GetArrayOf<TItem>(string keyPath)
        {
            List<Object> list;
            if(!TryGet(keyPath, out list))
                return new TItem[0];

            var result = new TItem[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                result[i] = (TItem) Convert.ChangeType(list[i], typeof (TItem));
            }
            return result;
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
            if (!TryGet(keyPath, out value))
            {
                throw new ArgumentException(string.Format("DynamicConfig keypath '{0}' is invalid. {1}Registered keys:{1}{2}", keyPath, Environment.NewLine, string.Join(Environment.NewLine, AllKeys)));
            }
            return value;
        }
    }
}
