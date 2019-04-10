using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DynamicConfig;
using DynamicConfigTokenizer.YML;
using GeCon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GeCon.Controllers
{
    [Route("configs/")]
    public class ConfigsController : Controller
    {
        private readonly string _basePath;

        public ConfigsController(IConfiguration configuration)
        {
            _basePath = configuration.GetValue<string>("DirectoryWithConfigs");
        }
        
        // GET configs/parts
        [HttpGet("parts")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
        public JsonResult Get()
        {
            var map = from fullDirName in Directory.GetDirectories(_basePath)
                let relativeDir = Path.GetFileName(fullDirName)
                let filesInDir = Directory.GetFiles(fullDirName, "*.yml")
                    .Where(f => (System.IO.File.GetAttributes(f) & FileAttributes.Hidden) == 0)
                    .Select(f => GetSectionPart(f))
                    .Where(p => p != null)
                    .OrderBy(p => p.display_name).ToArray()
                orderby relativeDir
                select new PartSection
                {
                    id = relativeDir,
                    display_name = Regex.Match(relativeDir, @"[^\d\-]+").Value,
                    parts = filesInDir
                };

            return Json(new PartsResponse {sections= map.ToArray()}, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        // GET /configs/combine/10-Base=common.yml;40-Stage=bingo-local.part.yml
        [HttpGet("combine/{parts}")]
        public ContentResult Combine(string parts)
        {
            return CombineInternal(parts, false);
        }
        
        [HttpGet("combineJson/{parts}")]
        public ContentResult CombineJson(string parts)
        {
            return CombineInternal(parts, true);
        }

        private ContentResult CombineInternal(string parts, bool isJson)
        {
            var parsedArguments = parts
                .Split(';', ',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split('=', ':'));

            Dictionary<object, object> currentMap = new Dictionary<object, object>();

            foreach (var arr in parsedArguments.OrderBy(arr => arr[0]))
            {
                try
                {
                    string filePath = Path.Combine(_basePath, arr[0], arr[1]);
                    var map = YamlProcessor.LoadYamlFile(filePath);
                    YamlProcessor.CopyAndMerge(map, currentMap);
                }
                catch (Exception e)
                {
                    throw new Exception($"Couldn't process config: {arr[0]}/{arr[1]}", e);
                }
            }

            currentMap["end"] = "end";

            return new ContentResult
            {
                ContentType = "text/x-yaml",
                Content = YamlProcessor.SerializeToString(currentMap, isJson),
            };
        }

        private Part GetSectionPart(string configFilePath)
        {
            try
            {
                string displayName = Path.GetFileNameWithoutExtension(configFilePath);

                using (var fileStream = System.IO.File.OpenRead(configFilePath))
                {
                    var config = DynamicConfigFactory.CreateConfig(new YmlDynamicConfigTokenizer(), fileStream);
                    config.Build(new DynamicConfigOptions());

                    if (config.TryGet("stage_data:server_version", out string value) && Regex.IsMatch(value, @"^\d\.\d{1,3}\.\d{1,3}-\w+$"))
                    {
                        displayName += $" [{value}]";
                    }
                }

                return new Part
                {
                    id = Path.GetFileName(configFilePath),
                    display_name = displayName,
                };
            }
            catch (Exception e)
            {
                return new Part
                {
                    id = Path.GetFileName(configFilePath),
                    display_name = Path.GetFileName(configFilePath),
                    error = e.Message
                };
            }
        }

    }
}
