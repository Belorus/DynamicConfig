﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CommandLine;
using DynamicConfig;

namespace ConstantGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Options options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                var files = options.InputConfigPath.Split(',').Select(File.OpenRead).OfType<Stream>().ToArray();
                var config = DynamicConfigFactory.CreateConfig(files);
                config.SetPrefixes(options.Prefixes.Split(',',';'));
                config.Build();

                string code = GenerateCode(options.Namespace, options.ClassName, config.AllKeys);
                File.WriteAllText(options.OutputClassPath, code);

                string log = string.Format("Successfully generated class '{0}' with {1} keys", options.ClassName, config.AllKeys.Count());
                Console.WriteLine(log);
            }
            else
            {
                string helpText = CommandLine.Text.HelpText.AutoBuild(options).ToString();
                Console.WriteLine(helpText);
            }
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