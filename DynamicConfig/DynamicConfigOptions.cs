using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    public class DynamicConfigOptions
    {
        public IReadOnlyCollection<string> Prefixes;
        public Version AppVersion;
        public IComparer<Version> VersionComparer;
        public ISegmentChecker SegmentChecker;
        public bool IgnorePrefixes;
    }
}