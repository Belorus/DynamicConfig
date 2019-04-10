using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    internal partial class ConfigKey : IComparable<ConfigKey>
    {
        public int CompareTo(ConfigKey other)
        {
            var result = Prefix.CompareTo(other.Prefix);
            if (result == 0)
            {
                result = VersionRange.CompareTo(other.VersionRange);
                if (result == 0)
                {
                    result = Segment.CompareTo(other.Segment);
                }
            }
            return result;
        }

        public class KeyEqualityComparer : IEqualityComparer<ConfigKey>
        {
            private static readonly IEqualityComparer<ConfigKey> Instance = new KeyEqualityComparer();

            public static IEqualityComparer<ConfigKey> Comparer
            {
                get { return Instance; }
            }

            public bool Equals(ConfigKey x, ConfigKey y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return string.Equals(x.Key, y.Key);
            }

            public int GetHashCode(ConfigKey obj)
            {
                unchecked
                {
                    return (obj.Key != null ? obj.Key.GetHashCode() : 0);
                }
            }
        }

    }
}
