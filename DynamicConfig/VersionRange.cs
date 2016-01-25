using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    internal class VersionRange : IComparable<VersionRange>
    {
        private readonly Version _from;
        private readonly Version _to;
        private readonly bool _isEmpty;

        private static readonly Version MinVersion = new Version(0,0);
        private static readonly Version MaxVersion = new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue);
        public static readonly VersionRange Empty = new VersionRange();

        private VersionRange()
        {
            _from = MinVersion;
            _to = MaxVersion;
            _isEmpty = true;
        }

        private VersionRange(Version @from, Version to)
        {
            _from = @from;
            _to = to;
        }

        public bool InRange(Version version, IComparer<Version> comparer)
        {
            if (version == null)
                return _isEmpty;

            return comparer.Compare(_from, version) <= 0 && comparer.Compare(version, _to) <= 0;
        }

        public int CompareTo(VersionRange other)
        {
            int result = _from.CompareTo(other._from);
            if (result != 0)
                return result;

            return other._to.CompareTo(_to);
        }

        public override string ToString()
        {
            return _isEmpty ? string.Empty : string.Format("({0}-{1})", _from, _to);
        }

#region Parse
        public static bool TryParse(string str, out VersionRange result)
        {
            var versions = str.Split('-');
            if (versions.Length == 1)
            {
                Version fromTo;
                if (Version.TryParse(versions[0], out fromTo))
                {
                    result = new VersionRange(fromTo, fromTo);
                    return true;
                }
            }
            else if (versions.Length == 2)
            {
                Version from;
                Version to;

                if (TryParseVersion(versions[0], out from, MinVersion) && TryParseVersion(versions[1], out to, MaxVersion))
                {
                    result = new VersionRange(from, to);
                    return true;
                }
            }

            result = null;
            return false;
        }

        private static bool TryParseVersion(string str, out Version version, Version defaultValue)
        {
            if (string.IsNullOrEmpty(str))
            {
                version = defaultValue;
                return true;
            }
            return Version.TryParse(str, out version);
        }
#endregion
    }
}
