namespace UnitTestProject
{
    public class TestData
    {
        public enum CompressionMode
        {
            GZip,
            Deflate
        }

        public const string Config = @"
http:
    -IOS-timeout: 1000
    timeout: 2000

    -(1.0.0-2.0.0)-timeout: 3000
    -(3.0.0-4.0.0)-timeout: 4000
    -<0..10>-retry_count: 10
    -<11..40>-retry_count: 40
    retry_count: 50

    compression:
        enabled: true
        mode: gzip
        int_mode: 1
";

        public const string AddNewFields = @"
http:
    throttle_interval: 1000
";

        public const string OverrideCompressionWithInvalidStructure = @"
http:
    compression: false
";
    }
}
