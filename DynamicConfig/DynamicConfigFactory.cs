using System;
using System.IO;

namespace DynamicConfig
{
    public static class DynamicConfigFactory
    {
        public static IDynamicConfig CreateConfig(Version applicationVersion, params Stream[] configStreams)
        {
            return new DynamicConfig(applicationVersion, configStreams);
        }
    }
}
