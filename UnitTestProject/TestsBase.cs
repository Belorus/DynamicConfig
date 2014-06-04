using System.IO;
using System.Linq;
using System.Text;
using DynamicConfig;

namespace UnitTestProject
{
    public class TestsBase
    {
        protected IDynamicConfig CreateConfig(string[] cfg, string[] prefixes)
        {
            var config = DynamicConfigFactory.CreateConfig(cfg.Select(c => new MemoryStream(Encoding.UTF8.GetBytes(c))).Cast<Stream>().ToArray());
            config.SetPrefixes(prefixes);
            config.Build();
            return config;
        }

        protected internal IDynamicConfig CreateConfig(string cfg, string[] prefixes)
        {
            return CreateConfig(new[] {cfg}, prefixes);
        }

        protected string GenerateKey(string key, params string[] prefixes)
        {
            if (!prefixes.Any())
                return key;
            return
                DynamicConfig.DynamicConfig.PrefixSeparator +
                string.Join(DynamicConfig.DynamicConfig.PrefixSeparator.ToString(), prefixes) +
                DynamicConfig.DynamicConfig.PrefixSeparator +
                key;
        }
    }
}