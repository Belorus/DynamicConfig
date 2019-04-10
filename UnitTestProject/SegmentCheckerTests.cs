using System;
using DynamicConfig;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestOf(typeof(SegmentChecker))]
    public class SegmentCheckerTests
    {
        [TestCase("0..10", 0, true)]
        [TestCase("11..20", 20, true)]
        [TestCase("11..20", 15, true)]
        [TestCase("0..10", 11, false)]
        [TestCase("*", 0, true)]
        [TestCase("*", 99, true)]
        [TestCase("*", 50, true)]
        public void SegmentChecker_Check_Success(string segmentationString, int seed, bool result)
        {
            Segment.TryParse(segmentationString, out var segment);
            var checker = new SegmentChecker(seed);
            Assert.That(checker.Check(segment), Is.EqualTo(result));
        }

        [TestCase("0..10", false)]
        [TestCase("*", true)]
        public void DefaultOnlySegmentChecker_Check_Success(string segmentationString, bool result)
        {
            Segment.TryParse(segmentationString, out var segment);
            var checker = SegmentChecker.DefaultOnly;
            Assert.That(checker.Check(segment), Is.EqualTo(result));
        }

        [TestCase("0..10")]
        [TestCase("*")]
        public void NullSegmentChecker_Check_Success(string segmentationString)
        {
            Segment.TryParse(segmentationString, out var segment);
            var checker = SegmentChecker.Null;
            Assert.That(checker.Check(segment), Is.True);
        }

        [TestCase(-1)]
        [TestCase(101)]
        public void SegmentChecker_InvalidSeed_Exception(int seed)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new SegmentChecker(seed));
        }
    }
}