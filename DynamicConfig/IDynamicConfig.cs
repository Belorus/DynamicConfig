namespace DynamicConfig
{
    public interface IDynamicConfig
    {
        void SetPrefixes(params string[] prefixes);
        void InsertPrefix(int index, string prefix);
        void Build();
        T Get<T>(string keyPath);
    }
}