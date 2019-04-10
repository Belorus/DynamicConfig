using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using DynamicConfig;
using DynamicConfigTokenizer.JSON;
using DynamicConfigTokenizer.YML;

namespace ConstantGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var parseResult = Parser.Default.ParseArguments<Options>(args);
            
            if (parseResult.Tag == ParserResultType.Parsed)
            {
                Options options = parseResult.MapResult(o => o, _ => null);
                var files = options.InputConfigPath.Split(',').Select(File.OpenRead).OfType<Stream>().ToArray();
                var config = DynamicConfigFactory.CreateConfig(ResolveFormatTokenizer(options.InputConfigPath), files);

                var configOptions = new DynamicConfigOptions
                {
                    IgnorePrefixes = true,
                    VersionComparer = VersionComparer.Null,
                    SegmentChecker = SegmentChecker.Null
                };
                config.Build(configOptions);

                string code = GenerateCode(options.Namespace, options.ClassName, config.AllKeys);
                File.WriteAllText(options.OutputClassPath, code);

                string log = $"Successfully generated class '{options.ClassName}' with {config.AllKeys.Count()} keys";
                Console.WriteLine(log);
            }
            else
            {
                string helpText = CommandLine.Text.HelpText.AutoBuild(parseResult).ToString();
                Console.WriteLine(helpText);
            }
        }

        private static IDynamicConfigTokenizer ResolveFormatTokenizer(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if(fileInfo.Extension == SupportedFormats.Json)
            {
                return new JsonDynamicConfigTokenizer();
            }
            if(fileInfo.Extension == SupportedFormats.Yml)
            {
                return new YmlDynamicConfigTokenizer();
            }

            throw new NotSupportedException($"{fileInfo.Extension} not supported ");
        }

        private static string GenerateCode(string ns, string className, IEnumerable<string> allKeys)
        {
            var f = new ConstantClassGenerator(ns, className, allKeys.ToDictionary(GetConstantName, k => k));
            return  f.TransformText();
        }

        private static string GetConstantName(string s)                
        {
            var parts = s.Split(':');

            return string.Join("_", parts.Select(StringExtensions.ToPascalCase));
        }
    }
}
