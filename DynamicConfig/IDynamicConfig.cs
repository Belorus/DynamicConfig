using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    public interface IDynamicConfig
    {
        //void SetApplicationVersion(Version version, IComparer<Version> comparer);
        //void SetPrefixes(params string[] prefixes);
        //void InsertPrefix(int index, string prefix);
        void Build(DynamicConfigOptions options);
        T Get<T>(string keyPath);
        TItem[] GetArrayOf<TItem>(string keyPath);
        bool TryGet<T>(string keyPath, out T value);
        IEnumerable<string> AllKeys { get; }
    }
}