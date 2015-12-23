using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    internal class PrefixConfig
    {
        private readonly Dictionary<string, int> _prefixes;

        public readonly int Count;

        public PrefixConfig(List<string> prefixes)
        {
            if (prefixes.Count > 32)
                throw new NotSupportedException("Max 32 prefixes");

            _prefixes = new Dictionary<string, int>(prefixes.Count);
            for (var i = 0; i < prefixes.Count; i++)
            {
                _prefixes[prefixes[i]] = i;
            }
            Count = _prefixes.Count;
        }

        public int IndexOf(string prefix)
        {
            int index;
            if(_prefixes.TryGetValue(prefix, out index))
                return index;
            return -1;
        }

        public bool Contains(string prefix)
        {
            return _prefixes.ContainsKey(prefix);
        }
    }
}