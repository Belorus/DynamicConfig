using CommandLine;

namespace YamlMerger
{
    public class Options
    {
        [Option('i', "input", Required = true)]
        public string InputPath { get; set; }

        [Option('o', "output", Required = true)]
        public string OutputPath { get; set; }

        [Option('p', "pattern", Default = "*.part.yml")]
        public string Pattern { get; set; }
    }
}