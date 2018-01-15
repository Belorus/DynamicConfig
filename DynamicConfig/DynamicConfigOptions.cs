using System;
using System.Collections.Generic;

namespace DynamicConfig
{
    public class DynamicConfigOptions
    {
        public ICollection<string> Prefixes;
        public Version AppVersion;
        public IComparer<Version> VersionComparer;
        public bool IgnorePrefixes;
        public bool IgnoreVersions;
    }
}