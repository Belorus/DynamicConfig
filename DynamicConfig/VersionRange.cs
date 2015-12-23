using System;

namespace DynamicConfig
{
    internal class VersionRange
    {
        private readonly Version _from;
        private readonly Version _to;

        public static readonly VersionRange Empty = new VersionRange(null, null);

        private VersionRange(Version from, Version to)
        {
            _from = @from;
            _to = to;
        }

        public bool InRange(Version version)
        {
            if (_from == null || _to == null)
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

        public override string ToString()
        {
            return string.Format("[{0}-{1}]", _from, _to);
        }
    }
}
