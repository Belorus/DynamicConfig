using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GeCon.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GeCon.Controllers
{
    [Route("configs/")]
    public class ConfigsController : Controller
    {
        private readonly string _basePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "../../Data"));
        
        // GET configs/parts
        [HttpGet("parts")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
        public JsonResult Get()
        {
            var map = (from fullDirName in Directory.GetDirectories(_basePath)
                    let relativeDir = Path.GetFileName(fullDirName)
                    let filesInDir = Directory.GetFiles(fullDirName)
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray()
                    select (relativeDir, filesInDir))
                .ToDictionary(a => a.Item1, a => a.Item2);

            return Json(new PartsResponse {parts = map}, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }

        // GET configs/combine?parts=common,something,blablabla
        [HttpGet("combine/{parts}")]
        public string Get(string parts)
        {
            var parsedArguments = parts.Split(';', ',', ' ')
                .Select(s => s.Split('=',':'))
                .ToDictionary(arr => arr[0], arr => arr[1]);

            Dictionary<object, object> currentMap = new Dictionary<object, object>();

            foreach (var kvPair in parsedArguments)
            {
                string filePath = Path.ChangeExtension(Path.Combine(_basePath, kvPair.Key, kvPair.Value), ".part.yml");
                var map = YamlProcessor.LoadYamlFile(filePath);
                YamlProcessor.CopyAndMerge(map, currentMap);
            }

            currentMap["end"] = "end";

            return YamlProcessor.SerializeToString(currentMap, false);
        }

    }
}
