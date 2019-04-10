using System;

namespace DynamicConfig
{
    public class SegmentChecker : ISegmentChecker
    {
        public static readonly ISegmentChecker DefaultOnly = new DefaultOnlySegmentChecker();
        public static readonly ISegmentChecker Null = new NullSegmentChecker();

        private readonly int _seed;

        public SegmentChecker(int seed)
        {
            if (0 > seed || seed > 99)
            {
                throw new ArgumentOutOfRangeException($"Argument {nameof(seed)} must be between 0 and 99");
            }
            _seed = seed;
        }

        public bool Check(Segment segment)
        {
            if (segment.IsDefaultSegment)
            {
                return true;
            }

            return segment.From <= _seed && _seed <= segment.To;
        }

        private sealed class DefaultOnlySegmentChecker : ISegmentChecker
        {
            public bool Check(Segment segment)
            {
                return segment.IsDefaultSegment;
            }
        }

        private sealed class NullSegmentChecker : ISegmentChecker
        {
            public bool Check(Segment segment)
            {
                return true;
            }
        }
    }
}
