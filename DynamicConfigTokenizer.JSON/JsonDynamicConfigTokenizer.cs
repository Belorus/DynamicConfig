using DynamicConfig;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DynamicConfigTokenizer.JSON
{
    public class JsonDynamicConfigTokenizer : IDynamicConfigTokenizer
    {
        private readonly Dictionary<JTokenType, Func<JToken, object>> _conversionTypesTable;

        public JsonDynamicConfigTokenizer()
        {
            _conversionTypesTable = new Dictionary<JTokenType, Func<JToken, object>>
            {
                { JTokenType.Boolean, t => t.Value<bool>() },
                { JTokenType.Float, t => t.Value<float>() },
                { JTokenType.String, t => t.Value<string>() },
                { JTokenType.Integer, t => t.Value<int>() }
            };
        }

        public Dictionary<object, object> Tokenize(Stream stream)
        {
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, 1024, true))
            {
                var result = Parse(JObject.Parse(reader.ReadToEnd()));
                return result as Dictionary<object, object>;
            }
        }

        private object ChangeType(JToken token)
        {
            if (_conversionTypesTable.ContainsKey(token.Type))
            {
                return _conversionTypesTable[token.Type](token);
            }

            throw new DynamicConfigException($"Can't parse token with type:{token.Type} by path:{token.Path}");
        }

        private object Parse(JToken token)
        {
            if (token.Type == JTokenType.Object)
            {
                var result = new Dictionary<object, object>();
                var obj = (JObject)token;
                foreach (var prop in obj.Properties())
                {
                    result.Add(prop.Name, Parse(prop.Value));
                }

                return result;
            }
            else if (token.Type == JTokenType.Array)
            {
                var arrayToken = (JArray)token;
                var list = new List<object>(arrayToken.Count);
                foreach (var listItem in arrayToken)
                {
                    list.Add(Parse(listItem));
                }

                return list;
            }
            else
            {
                return ChangeType(token);
            }
        }
    }
}
