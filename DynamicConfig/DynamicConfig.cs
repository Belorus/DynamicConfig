using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DynamicConfig
{ 
    internal class DynamicConfig : IDynamicConfig
    {
        private readonly List<Dictionary<object, object>> _configs;

        private Dictionary<string, object> _config;

        public DynamicConfig(IDynamicConfigTokenizer tokenizer, params Stream[] configs)
        {
            _configs = new List<Dictionary<object, object>>(configs.Length);
            foreach (var config in configs)
            {
                var dict = tokenizer.Tokenize(config);
                if (dict != null)
                    _configs.Add(dict);
            }
        }

        public void Build(DynamicConfigOptions options)
        {
            IPrefixBuilder prefixBuilder = options.IgnorePrefixes ? (IPrefixBuilder)new EmptyPrefixBuilder() : new PrefixBuilder(options.Prefixes);
            IComparer<Version> versionComparer = options.VersionComparer ?? VersionComparer.Default;
            ISegmentChecker segmentChecker = options.SegmentChecker ?? SegmentChecker.DefaultOnly;

            var configReader = new ConfigReader(_configs, prefixBuilder, options.AppVersion, versionComparer ,segmentChecker);

            _config = configReader.ParseConfig();
        }

        public IEnumerable<string> AllKeys => _config.Keys;

        public T Get<T>(string keyPath)
        {
            return ChangeType<T>(Get(keyPath));
        }

        public TItem[] GetArrayOf<TItem>(string keyPath)
        {
            if (TryGet(keyPath, out List<object> list))
            {
                var result = new TItem[list.Count];
                for (var i = 0; i < list.Count; i++)
                {
                    result[i] = ChangeType<TItem>(list[i]);
                }

                return result;
            }
            else
            {
                return new TItem[0];
            }
        }

        public bool TryGet<T>(string keyPath, out T value)
        {
            if (TryGet(keyPath, out var v))
            {
                value = ChangeType<T>(v);
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
            if (TryGet(keyPath, out var value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException(string.Format(
                    "DynamicConfig keypath '{0}' is invalid. {1}Registered keys:{1}{2}", keyPath, Environment.NewLine,
                    string.Join(Environment.NewLine, AllKeys)));
            }
        }

        private T ChangeType<T>(object value)
        {
            var type = typeof(T);
            if (type.GetTypeInfo().IsEnum)
            {
                if (value is string strValue)
                {
                    return (T) Enum.Parse(type, strValue, ignoreCase: true);
                }
                if (value is int)
                {
                    return (T) Enum.ToObject(type, value);
                }
            }
            return (T) Convert.ChangeType(value, typeof(T));
        }
    }
}
