using System;
using DynamicConfig;
using NUnit.Framework;

namespace UnitTestProject
{
    [TestOf(typeof(VersionRange))]
    public class VersionRangeTests
    {
        [TestCase("1.0.0-2.0.0", "1.5", true)]
        [TestCase("1.0.0-", "1.5", true)]
        [TestCase("-2.0.0", "1.5", true)]
        [TestCase("2.0.0", "1.5", false)]
        [TestCase("1.0-2.0", "1.0", true)]
        [TestCase("1.0.0-2.0.0", "1.0.0", true)]
        [TestCase("1.0.0.0-2.0.0.0", "1.0.0.0", true)]
        [TestCase("1.0-2.0", "1.0.0", true)]
        [TestCase("1.0.0-2.0.0", "1.0", false)]
        [TestCase("1.0.0-2.0.0", "1.0.0.0", true)]
        [TestCase("1.0.0.0-2.0.0.0", "1.0.0", false)]
        [TestCase("2.0.0", "2.0.0", true)]
        [TestCase("2.0.0", "2.0", false)]
        public void InRange_DefaultComparer_Result(string range, string targetVersion, bool result)
        {
            var version = Version.Parse(targetVersion);
            VersionRange.TryParse(range, out var versionRange);

            Assert.IsNotNull(versionRange);
            Assert.AreEqual(result, versionRange.InRange(version, VersionComparer.Default));
        }

        [TestCase("1.0-2.0", "1.0", true)]
        [TestCase("1.0.0-2.0.0", "1.0.0", true)]
        [TestCase("1.0.0.0-2.0.0.0", "1.0.0.0", true)]
        [TestCase("1.0-2.0", "1.0.0", true)]
        [TestCase("1.0.0-2.0.0", "1.0", true)]
        [TestCase("1.0.0-2.0.0", "1.0.0.0", true)]
        [TestCase("1.0.0.0-2.0.0.0", "1.0.0", true)]
        public void InRange_WeakComparer_Result(string range, string targetVersion, bool result)
        {
            var version = Version.Parse(targetVersion);
            VersionRange.TryParse(range, out var versionRange);

            Assert.AreEqual(result, versionRange.InRange(version, VersionComparer.Weak));
        }

        [TestCase("5.0-10.0", "1.0-10.0")]
        [TestCase("5.0-10.0", "1.0-4.0")]
        [TestCase("5.0-10.0", "4.0-9.0")]
        [TestCase("5.0-10.0", "4.0-11.0")]
        [TestCase("5.0-10.0", "-4.0")]
        [TestCase("5.0-10.0", "-11.0")]
        [TestCase("5.0-10.0", "-10.0")]
        public void VersionRange_Compare_LeftMoreThanRight(string left, string right)
        {
            VersionRange.TryParse(left, out var leftVersion);
            VersionRange.TryParse(right, out var rightVersion);

            Assert.IsTrue(leftVersion.CompareTo(rightVersion) > 0);
            Assert.IsTrue(rightVersion.CompareTo(leftVersion) < 0);
        }
    }
}
