using CommandLine;

namespace YamlMerger
{
    public class Options
    {
        [Option("i", Required = true)]
        public string InputPath { get; set; }

        [Option("o", Required = true)]
        public string OutputPath { get; set; }

        [Option("p", DefaultValue = "*.part.yml")]
        public string Pattern { get; set; }
    }
}