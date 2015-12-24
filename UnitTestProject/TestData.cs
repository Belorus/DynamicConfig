namespace UnitTestProject
{
    public class TestData
    {
        public const string EmptyData = "";

        public const string SimpleData = @"
a:
    b: test
";

        public const string SimpleDataExtension = @"
a:
    c: test2
";

        public const string Data2 = @"
a:
    b: a-b
    a: a-a
-a-a:
    b: -a-a-b
    a:
        c:
            e: -a-a-a-c-e
        d:
            f: -a-a-a-d-f
c:
    k1: c-k1
d:
    k1: d-k1
-a-d:
    k3: -a-d-k3
-b-d:
    k4: -b-d-k4
";

        public const string Data3 = @"
-a-c:
    k1: -a-c-k1
r:
    k5: r-k5
";

        public const string Data4 = @"
-a-c:
    k: -a-c-k
r:
    k: r-k
-np-r:
    k: -np-r-k
-a-np-r:
    k: -a-np-r-k
";

        public const string DataWithoutRoot = @"
key1: key1
key2: key2
key3: key3
";

        public const string DataWithVersions1 = @"
-(0.5.0-0.6.0)-key1: value1
key1               : value3
-(0.5.0-2.0.0)-key1: value2
";
        public const string DataWithVersions2 = @"
-(0.6.0-1.6.0)-key1: value1
key1               : value3
-(0.5.0-2.0.0)-key1: value2
";

        public const string DataWithVersions3 = @"
-(0.5.0-1.6.0)-key1: value1
key1               : value3
-(0.5.0-2.0.0)-key1: value2
";
    }
}
