using System.IO;

namespace DynamicConfig
{
    public static class DynamicConfigFactory
    {
        public static IDynamicConfig CreateConfig(params Stream[] configStreams)
        {
            return new DynamicConfig(configStreams);
        }
    }
}
