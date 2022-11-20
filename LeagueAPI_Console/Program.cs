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
            RealDelayer delayer = new();
            LeagueAPIClient client = new(http, parameters.Token, delayer, logger);
            DdragonRepository repo = new(client);
            RealDateTimeProvider dateTimeProvider = new();
            ExcelPrinter printer = new();

            MatchSaver matchSaver = new(
                fileIo,
                repo,
                printer,
                logger,
                dateTimeProvider
            );

            MatchCollectorEventHandler matchCollectorEventHandler = new(fileIo, matchSaver, parameters.OutputDirectory);
            MatchCollector collector = new(client, logger, matchCollectorEventHandler);

            FullRunner runner = new(
                repo,
                fileIo,
                logger,
                collector,
                matchSaver,
                client
            );
            await runner.DoFullRun(parameters);

            logger.Log("Press any key to exit.");
            Console.ReadKey();
        }
    }
}