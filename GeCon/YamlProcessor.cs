using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SharpYaml.Serialization;

namespace GeCon
{
    public static class YamlProcessor
    {
        public static void CopyAndMerge(
            Dictionary<object, object> from,
            Dictionary<object, object> to)
        {
            foreach (var kv in from)
            {
                object toValue;
                if (to.TryGetValue(kv.Key, out toValue))
                {
                    var toAsMap = toValue as Dictionary<object, object>;
                    if (toAsMap != null)
                    {
                        var fromAsMap = kv.Value as Dictionary<object, object>;
                        if (fromAsMap != null)
                        {
                            CopyAndMerge(fromAsMap, toAsMap);
                        }
                        else
                        {
                            toAsMap[kv.Key] = kv.Value;
                        }
                    }
                }
                else
                {
                    to[kv.Key] = kv.Value;
                }
            }
        }

        public static string SerializeToString(Dictionary<object, object> partMap, bool isJsonCompatible)
        {
            var stringBuilder = new StringBuilder();
            using (var writer = new StringWriter(stringBuilder))
            {
                new Serializer(new SerializerSettings()
                {
                    SortKeyForMapping = false,
                    EmitTags = false,
                    PreferredIndent = 4,
                    LimitPrimitiveFlowSequence = 25,
                    EmitJsonComptible = isJsonCompatible,
                    EmitAlias = false
                }).Serialize(writer, partMap);

                return stringBuilder.ToString();
            }
        }


        public static Dictionary<object, object> LoadYamlFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var formatter = new Serializer();
                return formatter.Deserialize<Dictionary<object, object>>(stream);
            };
        }
    }
}
