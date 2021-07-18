using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueAPI_Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string parametersPath = "";
            foreach (string arg in args)
            {
                Match match = Regex.Match(arg, "parametersPath-(.*)");
                if (match.Success) parametersPath = match.Groups[1].Value;
            }
            Parameters parameters = File.ReadAllText(parametersPath).DeserializeObject<Parameters>();
            RealHttpClient http = new();
            Delayer delayer = new();
            Logger_Console logger = new();
            LeagueAPIClient client = new(http, parameters.Token, delayer, logger);
            MatchCollector collector = new(client, logger);
            DdragonRepository repo = new(parameters.DdragonJsonFilesDirectoryPath);
            FullRunner runner = new(collector, repo);
            List<string> files = await runner.DoFullRun(parameters.OutputDirectory, parameters.QueueId, parameters.AccountPuuid, targetVersion: parameters.TargetVersion, maxCount: parameters.MaxCount);
            if (files.Count > 0) logger.Log($"{files.Count} files written at {parameters.OutputDirectory}. Press any key to exit." );
            Console.ReadKey();
        }
    }
}
