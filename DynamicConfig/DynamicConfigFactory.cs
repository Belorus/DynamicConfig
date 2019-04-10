using System.IO;

namespace DynamicConfig
{
    public static class DynamicConfigFactory
    {
        public static IDynamicConfig CreateConfig(IDynamicConfigTokenizer tokenizer, params Stream[] configStreams)
        {
            return new DynamicConfig(tokenizer, configStreams);
        }
    }
}
