using System;

namespace DynamicConfig
{
    internal class VersionRange : IComparable<VersionRange>
    {
        private readonly Version _from;
        private readonly Version _to;
        private readonly bool _isEmpty;

        public static readonly VersionRange Empty = new VersionRange(null, null);

        private VersionRange(Version from, Version to)
        {
            _from = @from;
            _to = to;

            _isEmpty = _from == null && _to == null;
        }

        public bool InRange(Version version)
        {
            if (_isEmpty)
                return true;
            
            if (!_isEmpty && version == null)
                return false;

            return _from <= version && version <= _to;
        }

        public static VersionRange Parse(string str)
        {
            var versions = str.Split('-');
            if (versions.Length == 2)
            {
                Version from;
                Version to;
                if (Version.TryParse(versions[0], out from) && Version.TryParse(versions[1], out to))
                {
                    return new VersionRange(from, to);
                }
            }

            throw new ArithmeticException("Unsupported verion range format");
        }

        public int CompareTo(VersionRange other)
        {
            if (_isEmpty && other._isEmpty)
                return 0;

            if (_isEmpty && !other._isEmpty)
                return -1;

            if (!_isEmpty && other._isEmpty)
                return 1;

            int result = _from.CompareTo(other._from);
            if (result != 0)
                return result;

            return other._to.CompareTo(_to);
        }

        public override string ToString()
        {
            return string.Format("[{0}-{1}]", _from, _to);
        }
    }
}
