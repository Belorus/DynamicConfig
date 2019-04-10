using System.Collections.Generic;

namespace DynamicConfig
{
    public interface IDynamicConfig
    {
        void Build(DynamicConfigOptions options);
        T Get<T>(string keyPath);
        TItem[] GetArrayOf<TItem>(string keyPath);
        bool TryGet<T>(string keyPath, out T value);
        IEnumerable<string> AllKeys { get; }
    }
}