using System;

namespace DynamicConfig
{
    public class DynamicConfigException : Exception 
    {
        public DynamicConfigException()
        {
        }

        public DynamicConfigException(string message) : base(message)
        {
        }

        public DynamicConfigException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
