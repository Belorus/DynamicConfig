using DynamicConfig;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestOf(typeof(Segment))]
    public class SegmentTests
    {
        [TestCase("0..10", 0, 10, false)]
        [TestCase("20..30", 20, 30, false)]
        [TestCase("91..99", 91, 99, false)]
        [TestCase("0..99", 0, 99, false)]
        [TestCase("*", 0, 100, true)]
        public void TryParse_ValidData_Success(string segmentationString, int from, int to, bool isDefaultSegment)
        {
            var parseResult = Segment.TryParse(segmentationString, out var segmentation);
            Assert.IsTrue(parseResult);
            Assert.IsNotNull(segmentation);

            if (!isDefaultSegment)
            {
                Assert.AreEqual(from, segmentation.From);
                Assert.AreEqual(to, segmentation.To);
                Assert.IsFalse(segmentation.IsDefaultSegment);
            }
            else
            {
                Assert.IsTrue(segmentation.IsDefaultSegment);
                Assert.Throws<DynamicConfigException>(() =>
                {
                    var segmentationFrom = segmentation.From;
                });
                Assert.Throws<DynamicConfigException>(() =>
                {
                    var segmentationTo = segmentation.To;
                });
            }
        }

        [TestCase("-1..10")]
        [TestCase("90..100")]
        [TestCase("90..101")]
        [TestCase("-1..101")]
        [TestCase("30..20")]
        [TestCase("text..text")]
        [TestCase("1..2..")]
        [TestCase("..1..2")]
        public void TryParse_InvalidData_Failed(string segmentationString)
        {
            var parseResult = Segment.TryParse(segmentationString, out var segmentation);
            Assert.IsFalse(parseResult);
            Assert.IsNull(segmentation);
        }

        [TestCase("11..20", "*")]
        [TestCase("11..20", "0..10")]
        [TestCase("11..20", "0..20")]
        [TestCase("11..20", "11..30")]
        [TestCase("11..20", "0..10")]
        public void Segmentation_Compare_LeftMoreThanRight(string left, string right)
        {
            Segment.TryParse(left, out var leftSegment);
            Segment.TryParse(right, out var rightSegment);

            Assert.IsTrue(leftSegment.CompareTo(rightSegment) > 0);
            Assert.IsTrue(rightSegment.CompareTo(leftSegment) < 0);

        }
    }
}