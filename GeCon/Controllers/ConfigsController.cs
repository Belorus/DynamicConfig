using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
                let filesInDir = Directory.GetFiles(fullDirName)
                        .Select(f => new Part
                    {
                        id = Path.GetFileNameWithoutExtension(f),
                        display_name = Path.GetFileName(f),
                    }).ToArray()
                select new PartSection()
                {
                    id = relativeDir,
                    display_name = Regex.Match(relativeDir, @"[^\d\-]+").Value,
                    parts = filesInDir
                };

            return Json(new PartsResponse {sections= map.ToArray()}, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }

        // GET /configs/combine/10-Base=common.yml;40-Stage=bingo-local.part.yml
        [HttpGet("combine/{parts}")]
        public ContentResult Get(string parts)
        {
            var parsedArguments = parts.Split(';', ',')
                .Select(s => s.Split('=',':'))
                .ToDictionary(arr => arr[0], arr => arr[1]);

            Dictionary<object, object> currentMap = new Dictionary<object, object>();

            foreach (var kvPair in parsedArguments)
            {
                string filePath = Path.Combine(_basePath, kvPair.Key, kvPair.Value);
                var map = YamlProcessor.LoadYamlFile(filePath);
                YamlProcessor.CopyAndMerge(map, currentMap);
            }

            currentMap["end"] = "end";

            return new ContentResult {
                ContentType = "text/x-yaml",
                Content = YamlProcessor.SerializeToString(currentMap, false),
            }; 
        }

    }
}
