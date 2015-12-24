using System;
using DynamicConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
    [TestClass]
    public class VersionRangeTests
    {
        [TestMethod]
        public void ParseVersionTest()
        {
            // Act
            string version1 = "1.0.0-2.0.0";
            string version2 = "1.0.0-";
            string version3 = "-2.0.0";
            string version4 = "2.0.0";

            VersionRange versionRange1;
            VersionRange versionRange2;
            VersionRange versionRange3;
            VersionRange versionRange4;

            // Arrange

            bool result1 = VersionRange.TryParse(version1, out versionRange1);
            bool result2 = VersionRange.TryParse(version2, out versionRange2);
            bool result3 = VersionRange.TryParse(version3, out versionRange3);
            bool result4 = VersionRange.TryParse(version4, out versionRange4);

            // Assert
            Assert.IsTrue(result1);
            Assert.IsTrue(result2);
            Assert.IsTrue(result3);
            Assert.IsTrue(result4);

            Assert.IsNotNull(versionRange1);
            Assert.IsNotNull(versionRange2);
            Assert.IsNotNull(versionRange3);
            Assert.IsNotNull(versionRange4);
        }

        [TestMethod]
        public void InRangeTest()
        {
            // Act
            var version = Version.Parse("1.5");
            string version1 = "1.0.0-2.0.0";
            string version2 = "1.0.0-";
            string version3 = "-2.0.0";
            string version4 = "2.0.0";

            VersionRange versionRange1;
            VersionRange versionRange2;
            VersionRange versionRange3;
            VersionRange versionRange4;

            // Arrange
            VersionRange.TryParse(version1, out versionRange1);
            VersionRange.TryParse(version2, out versionRange2);
            VersionRange.TryParse(version3, out versionRange3);
            VersionRange.TryParse(version4, out versionRange4);

            // Assert
            Assert.IsTrue(versionRange1.InRange(version));
            Assert.IsTrue(versionRange2.InRange(version));
            Assert.IsTrue(versionRange3.InRange(version));
            Assert.IsFalse(versionRange4.InRange(version));
            Assert.IsTrue(versionRange4.InRange(Version.Parse("2.0.0")));
        }
    }
}
