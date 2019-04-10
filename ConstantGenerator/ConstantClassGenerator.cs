using System.Collections.Generic;
using System.Text;

namespace ConstantGenerator
{
    internal class ConstantClassGenerator
    {
        private readonly string _ns;
        private readonly string _className;
        private readonly IReadOnlyDictionary<string, string> _map;

        public ConstantClassGenerator(string @namespace, string className, IReadOnlyDictionary<string, string> map)
        {
            _ns = @namespace;
            _className = className;
            _map = map;
        }

        public string TransformText()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {_ns}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class {_className}");
            sb.AppendLine( "    {");
            foreach (var kv in _map)
            {
                sb.AppendLine($"        public const string {kv.Key} = \"{kv.Value}\";");
            }
            sb.AppendLine( "    }");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
