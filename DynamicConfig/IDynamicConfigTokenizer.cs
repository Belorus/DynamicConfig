using System.Collections.Generic;
using System.IO;

namespace DynamicConfig
{
    public interface IDynamicConfigTokenizer
    {
        Dictionary<object, object> Tokenize(Stream steam);
    }
}
