using System;

namespace DynamicConfig
{
    internal class Prefix : IComparable<Prefix>
    {
        private readonly int _hash;

        public int Hash
        {
            get { return _hash; }
        }

        private Prefix(int hash)
        {
            _hash = hash;
        }

        public Prefix(PrefixConfig prefixConfig, params string[] prefixes)
        {
            foreach (var prefix in prefixes)
            {
                int offset = prefixConfig.GetIndex(prefix);
                if (offset < 0)
                    throw new NotSupportedException(string.Format("Prefix \"{0}\" doesn't supported", prefix));

                _hash |= 1 << prefixConfig.Count - offset - 1;
            }            
        }

        public Prefix Merge(Prefix other)
        {
            return Merge(this, other);
        }

        public static Prefix Merge(Prefix first, Prefix second)
        {
            return new Prefix(first._hash | second._hash);
        }

        protected bool Equals(Prefix other)
        {
            return _hash == other._hash;
        }

        public int CompareTo(Prefix other)
        {
            int thisBitsCount = _hash.BitsCount();
            int otherBitsCount = other._hash.BitsCount();
            if(thisBitsCount == otherBitsCount)
                return _hash.CompareTo(other.Hash);
            return thisBitsCount.CompareTo(otherBitsCount);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Prefix)obj);
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override string ToString()
        {
            return Convert.ToString(_hash, 2);
        }
    }
}