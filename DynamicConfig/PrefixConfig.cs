using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    internal class PrefixConfig
    {
        private readonly Dictionary<string, int> _prefixes; 

        public PrefixConfig(params string[] prefixes)
        {
            if (prefixes.Length > 32)
                throw new NotSupportedException("Max 32 prefixes");

            _prefixes = new Dictionary<string, int>(prefixes.Length);
            for (var i = 0; i < prefixes.Length; i++)
            {
                _prefixes[prefixes[i]] = i;
            }
        }

        public int Count
        {
            get { return _prefixes.Count; }
        }

        public int GetIndex(string prefix)
        {
            int index;
            if(_prefixes.TryGetValue(prefix, out index))
                return index;
            return -1;
        }
    }
}