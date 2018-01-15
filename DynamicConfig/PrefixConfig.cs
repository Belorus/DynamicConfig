using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    internal interface IPrefixBuilder
    {
        bool Contains(string prefix);
        Prefix Create(List<string> prefixes);
    }

    internal class PrefixBuilder : IPrefixBuilder
    {
        private readonly Dictionary<string, int> _prefixes;
        private readonly int _count;

        public PrefixBuilder(ICollection<string> prefixes)
        {
            if (prefixes != null)
            {
                if (prefixes.Count > 32)
                    throw new NotSupportedException("Max 32 prefixes");

                _prefixes = new Dictionary<string, int>(prefixes.Count);
                int index = 0;
                foreach (var prefix in prefixes)
                {
                    _prefixes[prefix] = index;
                    index++;
                }
                _count = _prefixes.Count;
            }
            else
            {
                _prefixes = new Dictionary<string, int>(0);
                _count = 0;
            }
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