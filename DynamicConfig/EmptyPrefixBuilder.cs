using System.Collections.Generic;

namespace DynamicConfig
{
    internal class EmptyPrefixBuilder : IPrefixBuilder
    {
        public bool Contains(string prefix)
        {
            return true;
        }

        public Prefix Create(IReadOnlyCollection<string> prefixes)
        {
            return Prefix.Empty;
        }
    }
}