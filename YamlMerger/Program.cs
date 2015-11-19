using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using CommandLine.Text;
using SharpYaml.Serialization;

namespace YamlMerger
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = new Options();
            if (Parser.Default.ParseArguments(args, options))
            {
                string packagePath = Path.Combine(options.OutputPath, "package.zip");

                Directory.CreateDirectory(options.InputPath);
                Directory.CreateDirectory(options.OutputPath);
                File.Delete(packagePath);

                Console.WriteLine("Creating DynamicConfigs...");
                Console.WriteLine("Pattern for config files is " + options.Pattern);

                string commonYamlPath = Path.Combine(options.InputPath, "common.yml");

                foreach (string partFilePath in Directory.GetFiles(options.InputPath, options.Pattern))
                {
                    Dictionary<object, object> commonMap = LoadYamlFile(commonYamlPath);
                    Dictionary<object, object> partMap = LoadYamlFile(partFilePath);

                    Dictionary<object, object> resultMap = partMap.ToDictionary(kv => kv.Key, kv => kv.Value);
                    CopyAndMerge(commonMap, resultMap);

                    string outFilePath = Path.Combine(options.OutputPath, Path.GetFileName(partFilePath).Replace(".part", ""));

                    SaveYamlToFile(resultMap, outFilePath);
                  
                }
            }
            else
            {
                var helpText = HelpText.AutoBuild(options).ToString();

                Console.WriteLine(helpText);
            }
        }

        private static void CopyAndMerge(
            Dictionary<object, object> from,
            Dictionary<object, object> to)
        {
            foreach (var kv in from)
            {
                object toValue;
                if (to.TryGetValue(kv.Key, out toValue))
                {
                    var toAsMap = toValue as Dictionary<object, object>;
                    if (toAsMap != null)
                    {
                        var fromAsMap = kv.Value as Dictionary<object, object>;
                        if (fromAsMap != null)
                        {
                            CopyAndMerge(fromAsMap, toAsMap);
                        }
                        else
                        {
                            toAsMap[kv.Key] = kv.Value;
                        }
                    }
                    else
                    {
                        throw new NotSupportedException(string.Format("Merging two scalar keys [{0}] to associative array is not supported", kv.Key));
                    }
                }
                else
                {
                    to[kv.Key] = kv.Value;
                }
            }
        }

        private static void SaveYamlToFile(Dictionary<object, object> partMap, string outFilePath)
        {
            using (var file = File.OpenWrite(outFilePath))
            {
                new Serializer(new SerializerSettings() {SortKeyForMapping = false, EmitTags = false, PreferredIndent = 4}).Serialize(file, partMap);
            }
        }

        private static Dictionary<object, object> LoadYamlFile(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var formatter = new Serializer();
                return formatter.Deserialize<Dictionary<object, object>>(stream);
            };
        }
    }
}