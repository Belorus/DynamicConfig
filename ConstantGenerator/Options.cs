using CommandLine;

namespace ConstantGenerator
{
    public class Options
    {
        [Option("i", Required = true)]
        public string InputConfigPath { get; set; }

        [Option("o", Required = true)]
        public string OutputClassPath { get; set; }

        [Option("n", Required = true)]
        public string Namespace { get; set; }

        [Option("c", Required = true)]
        public string ClassName { get; set; }
    }
}