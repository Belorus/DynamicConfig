using System;

namespace DynamicConfig
{
    public class Segment : IComparable<Segment>
    {
        internal static readonly Segment Default = new Segment();

        private readonly int _from;
        private readonly int _to;
        public int From
        {
            get
            {
                if (IsDefaultSegment)
                {
                    throw new DynamicConfigException("Is default segment");
                }

                return _from;
            }
        }

        public int To
        {
            get
            {
                if (IsDefaultSegment)
                {
                    throw new DynamicConfigException("Is default segment");
                }

                return _to;
            }
        }

        public readonly bool IsDefaultSegment;

        private Segment()
        {
            IsDefaultSegment = true;
            _from = -1;
            _to = int.MaxValue;
        }

        private Segment(int @from, int to)
        {
            IsDefaultSegment = false;
            _from = @from;
            _to = to;
        }

        public int CompareTo(Segment other)
        {
            int result = _from.CompareTo(other._from);
            if (result != 0)
                return result;

            return other._to.CompareTo(_to);
        }

        public static bool TryParse(string value, out Segment segment)
        {
            if (value == "*")
            {
                segment = Default;
                return true;
            }

            const string segmentDelimeter = "..";
            var index = value.IndexOf(segmentDelimeter, StringComparison.Ordinal);
            if (index > 0)
            {
                if (int.TryParse(value.Substring(0, index), out var from) && int.TryParse(value.Substring(index + segmentDelimeter.Length), out var to))
                {
                    if (0 <= from && from < to && to <= 99)
                    {
                        segment = new Segment(from, to);
                        return true;
                    }
                }
            }

            segment = null;
            return false;
        }

        public override string ToString()
        {
            return IsDefaultSegment ? "<*>" : $"<{_from}..{_to}>";
        }
    }
}
