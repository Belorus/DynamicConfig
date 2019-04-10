using DynamicConfig;
using SharpYaml.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DynamicConfigTokenizer.YML
{
    public class YmlDynamicConfigTokenizer : IDynamicConfigTokenizer
    {
        private readonly Serializer _formatter;

        public YmlDynamicConfigTokenizer()
        {
            _formatter = new Serializer(new SerializerSettings { Attributes = new EmptyAttributeRegistry(), EmitAlias = false });
        }

        public Dictionary<object, object> Tokenize(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
               return _formatter.Deserialize<Dictionary<object, object>>(reader);
            }
        }
    }
}
