using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    public class VersionComparer
    {
        public static IComparer<Version> Default = new DefaultVersionComparer();
        public static IComparer<Version> Weak = new WeakVersionComparer();
        public static IComparer<Version> Null = new NullVersionComparer();

        private class DefaultVersionComparer : IComparer<Version>
        {
            public int Compare(Version x, Version y)
            {
                if (x == null && y == null)
                    return 0;
                if (y == null)
                    return 1;
                if (x == null)
                    return -1;
                return x.CompareTo(y);    
            }
        }

        private class WeakVersionComparer : IComparer<Version>
        {
            public int Compare(Version a, Version b)
            {
                if (a == null && b == null)
                    return 0;
                if (b == null)
                    return 1;
                if (a == null)
                    return -1;

                a = new Version(a.Major == -1 ? 0 : a.Major, a.Minor == -1 ? 0 : a.Minor, a.Build == -1 ? 0 : a.Build, a.Revision == -1 ? 0 : a.Revision);
                b = new Version(b.Major == -1 ? 0 : b.Major, b.Minor == -1 ? 0 : b.Minor, b.Build == -1 ? 0 : b.Build, b.Revision == -1 ? 0 : b.Revision);
                return a.CompareTo(b);
            }
        }

        private class NullVersionComparer : IComparer<Version>
        {
            public int Compare(Version x, Version y)
            {
                return 0;
            }
        }
    }
}
