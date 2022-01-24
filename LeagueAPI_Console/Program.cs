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
            RealFileIO fileIO = new();
            RealHttpClient http = new();
            RealWebClient webClient = new();
            RealDelayer delayer = new();
            Logger_Console logger = new();
            LeagueAPIClient client = new(http, parameters.Token, delayer, logger);
            MatchCollector collector = new(client, logger);
            DdragonRepository repo = new(fileIO, parameters.DdragonJsonFilesDirectoryPath);
            ArchiveExtractor extractor = new();
            DdragonRepositoryUpdater repoUpdater = new(client, webClient, fileIO, logger, extractor, parameters.DdragonJsonFilesDirectoryPath);
            RealDateTimeProvider dateTimeProvider = new();
            RealGuidProvider guidProvider = new();
            ExcelPrinter printer = new();
            FullRunner runner = new(client, collector, repo, fileIO, dateTimeProvider, guidProvider, printer, logger, repoUpdater);
            List<string> files = await runner.DoFullRun(
                parameters.OutputDirectory,
                parameters.QueueId,
                parameters.AccountPuuid,
                targetVersions: parameters.RangeOfTargetVersions,
                maxCount: parameters.MaxCount,
                parameters.IncludeWinRatesForMinutes,
                parameters.ExistingMatchesFile,
                parameters.GetLatestDdragonData);
            if (files.Count > 0) logger.Log($"{files.Count} files written at {parameters.OutputDirectory}. Press any key to exit." );
            Console.ReadKey();
        }
    }
}
