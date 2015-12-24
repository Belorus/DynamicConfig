using System;

namespace DynamicConfig
{
    internal class Prefix : IComparable<Prefix>
    {
        public readonly int Hash;

        public static readonly Prefix Empty = new Prefix(0);

        public Prefix(int hash)
        {
            Hash = hash;
        }

        public Prefix Merge(Prefix other)
        {
            return Merge(this, other);
        }

        public static Prefix Merge(Prefix first, Prefix second)
        {
            return new Prefix(first.Hash | second.Hash);
        }

        protected bool Equals(Prefix other)
        {
            return Hash == other.Hash;
        }

        public int CompareTo(Prefix other)
        {
            int thisBitsCount = Hash.BitsCount();
            int otherBitsCount = other.Hash.BitsCount();
            if(thisBitsCount == otherBitsCount)
                return Hash.CompareTo(other.Hash);
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
            return Hash;
        }

        public override string ToString()
        {
            return Convert.ToString(Hash, 2);
        }
    }
}