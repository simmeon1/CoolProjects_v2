using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
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
            RealFileIO fileIo = new();
            RealHttpClient http = new();
            Logger_Console logger = new();
            RealWebClient webClient = new(logger);
            RealDelayer delayer = new();
            LeagueAPIClient client = new(http, parameters.Token, delayer, logger);
            DdragonRepository repo = new(fileIo, parameters.DdragonJsonFilesDirectoryPath);
            ArchiveExtractor extractor = new();
            DdragonRepositoryUpdater repoUpdater = new(
                client,
                webClient,
                fileIo,
                logger,
                extractor,
                parameters.DdragonJsonFilesDirectoryPath
            );
            RealDateTimeProvider dateTimeProvider = new();
            ExcelPrinter printer = new();

            List<string> parsedTargetVersions = await client.GetParsedListOfVersions(parameters.RangeOfTargetVersions);

            MatchSaver matchSaver = new(
                fileIo,
                repo,
                printer,
                logger,
                dateTimeProvider,
                parameters.OutputDirectory,
                parsedTargetVersions.ConcatenateListOfStringsToCommaString(),
                parameters.IncludeWinRatesForMinutes
            );

            MatchCollectorEventHandler matchCollectorEventHandler = new(fileIo, matchSaver, parameters.OutputDirectory);
            MatchCollector collector = new(client, logger, matchCollectorEventHandler);

            FullRunner runner = new(
                repo,
                fileIo,
                logger,
                repoUpdater,
                collector,
                matchSaver
            );
            await runner.DoFullRun(parameters, parsedTargetVersions);

            logger.Log("Press any key to exit.");
            Console.ReadKey();
        }
    }
}