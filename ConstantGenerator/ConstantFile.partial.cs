using System.Collections.Generic;

namespace ConstantGenerator
{
    public partial class ConstantClassGenerator
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
    }
}
