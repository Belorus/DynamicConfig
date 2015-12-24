using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    internal class PrefixBuilder
    {
        private readonly Dictionary<string, int> _prefixes;
        private readonly int _count;

        public PrefixBuilder(List<string> prefixes)
        {
            if (prefixes.Count > 32)
                throw new NotSupportedException("Max 32 prefixes");

            _prefixes = new Dictionary<string, int>(prefixes.Count);
            for (var i = 0; i < prefixes.Count; i++)
            {
                _prefixes[prefixes[i]] = i;
            }
            _count = _prefixes.Count;
        }

        private int IndexOf(string prefix)
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

        public Prefix Create(List<string> prefixes)
        {
            int hash = 0;
            foreach (var prefix in prefixes)
            {
                int offset = IndexOf(prefix);
                if (offset < 0)
                    throw new NotSupportedException(string.Format("Prefix \"{0}\" doesn't supported", prefix));

                hash |= 1 << _count - offset - 1;
            }
            return new Prefix(hash);
        }
    }
}