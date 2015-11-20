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
                Console.WriteLine("Merging dynamic configs...");

                Directory.CreateDirectory(options.InputPath);
                Directory.CreateDirectory(options.OutputPath);
                File.Delete(Path.Combine(options.OutputPath, "package.zip"));

                string commonYamlPath = Path.Combine(options.InputPath, "common.yml");

                string[] partsToProcess = Directory.GetFiles(options.InputPath, options.Pattern);

                Console.WriteLine("Processing {0} files...", partsToProcess.Length);

                foreach (string partFilePath in partsToProcess)
                {
                    Dictionary<object, object> commonMap = LoadYamlFile(commonYamlPath);
                    Dictionary<object, object> partMap = LoadYamlFile(partFilePath);

                    Dictionary<object, object> resultMap = partMap.ToDictionary(kv => kv.Key, kv => kv.Value);
                    CopyAndMerge(Path.GetFileNameWithoutExtension(partFilePath), commonMap, resultMap);

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
            string name,
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
                            CopyAndMerge(name, fromAsMap, toAsMap);
                        }
                        else
                        {
                            toAsMap[kv.Key] = kv.Value;
                        }
                    }
                    else
                    {
                        Console.WriteLine("[{0}] - Merging key [{1}]: Taking '{2}' instead of '{3}'", name, kv.Key, toValue, kv.Value);
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
            using (var file = File.CreateText(outFilePath))
            {
                new Serializer(new SerializerSettings() {SortKeyForMapping = false, EmitTags = false, PreferredIndent = 4, LimitPrimitiveFlowSequence = 15}).Serialize(file, partMap);
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