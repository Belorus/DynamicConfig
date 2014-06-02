using System.Collections.Generic;

namespace DynamicConfig
{
    internal partial class ConfigKey
    {
        public class StrictEqualityComparer : IEqualityComparer<ConfigKey>
        {
            private static readonly IEqualityComparer<ConfigKey> Instance = new StrictEqualityComparer();

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
                return string.Equals(x.Key, y.Key) && Equals(x.Prefix, y.Prefix);
            }

            public int GetHashCode(ConfigKey obj)
            {
                unchecked
                {
                    return ((obj.Key != null ? obj.Key.GetHashCode() : 0) * 397) ^ (obj.Prefix != null ? obj.Prefix.GetHashCode() : 0);
                }
            }
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
