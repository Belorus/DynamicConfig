using System.Collections.Generic;

namespace DynamicConfig
{
    public interface IDynamicConfig
    {
        void SetPrefixes(params string[] prefixes);
        void InsertPrefix(int index, string prefix);
        void Build();
        T Get<T>(string keyPath);
        bool TryGet<T>(string keyPath, out T value);
        IEnumerable<string> AllKeys { get; }
    }
}