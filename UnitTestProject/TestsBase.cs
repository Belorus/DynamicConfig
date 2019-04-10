using System.IO;
using System.Linq;
using System.Text;
using DynamicConfig;
using DynamicConfigTokenizer.YML;

namespace UnitTestProject
{
    public class TestsBase
    {
        protected IDynamicConfig CreateConfig(string[] cfg, string[] prefixes)
        {
            var config = DynamicConfigFactory.CreateConfig(new YmlDynamicConfigTokenizer(), cfg.Select(c => new MemoryStream(Encoding.UTF8.GetBytes(c))).Cast<Stream>().ToArray());
            config.Build(new DynamicConfigOptions{Prefixes = prefixes});
            return config;
        }

        protected IDynamicConfig CreateConfig(string cfg, string[] prefixes)
        {
            return CreateConfig(new[] {cfg}, prefixes);
        }

        protected string GenerateKey(string key, params string[] prefixes)
        {
            if (!prefixes.Any())
                return key;
            return
                ConfigKeyBuilder.PrefixSeparator +
                string.Join(ConfigKeyBuilder.PrefixSeparator.ToString(), prefixes) +
                ConfigKeyBuilder.PrefixSeparator +
                key;
        }
    }
}