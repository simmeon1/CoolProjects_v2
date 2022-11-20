using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Common_ClassLibrary;

namespace LeagueAPI_ClassLibrary
{
    public class MatchSaver : IMatchSaver
    {
        private readonly IFileIO fileIo;
        private readonly IDDragonRepository repository;
        private readonly IExcelPrinter excelPrinter;
        private readonly ILogger logger;
        private readonly IDateTimeProvider dateTimeProvider;
        private string outputDirectory;
        private string versionStr;

        public MatchSaver(
            IFileIO fileIo,
            IDDragonRepository repository,
            IExcelPrinter excelPrinter,
            ILogger logger,
            IDateTimeProvider dateTimeProvider
        )
        {
            this.fileIo = fileIo;
            this.repository = repository;
            this.excelPrinter = excelPrinter;
            this.logger = logger;
            this.dateTimeProvider = dateTimeProvider;
        }
        
        public void SetOutputDetails(string outputDirectory, string versionStr)
        {
            this.outputDirectory = outputDirectory;
            this.versionStr = versionStr;
        }

        public List<string> SaveMatches(
            List<LeagueMatch> matches
        )
        {
            List<string> createdFiles = new();

            string idString = $"_{versionStr}_{Globals.GetDateTimeFileNameFriendly(dateTimeProvider.Now())}";
            string path = Path.Combine(outputDirectory, $"Results{idString}");
            string matchesFilePath = Path.Combine(path, $"Matches{idString}.json");
            string matchesLinesFilePath = Path.Combine(path, $"MatchesLines{idString}.txt");
            string itemSetFilePath = Path.Combine(path, $"ItemSet_All{idString}.json");
            string statsFilePath = Path.Combine(path, $"Stats{idString}.xlsx");
            string logFilePath = Path.Combine(path, $"Log{idString}.txt");
            if (!fileIo.DirectoryExists(path)) fileIo.CreateDirectory(path);

            fileIo.WriteAllText(matchesFilePath, matches.SerializeObject());
            createdFiles.Add(matchesFilePath);

            StringBuilder sb = new("");
            foreach (LeagueMatch match in matches)
            {
                sb.AppendLine(match.SerializeObject());
            }

            fileIo.WriteAllText(matchesLinesFilePath, sb.ToString());
            createdFiles.Add(matchesLinesFilePath);
            
            DataCollector collector = new(repository);

            DataCollectorResults allMatchesData = collector.GetData(matches);

            ItemSetExporter exporter = new(repository);
            DataCollectorResults resultData = allMatchesData;
            List<TableEntry<Item>> itemData = resultData.GetItemData();
            string itemSetFileName = itemSetFilePath;
            string itemSetJson = exporter.GetItemSet(itemData, Path.GetFileNameWithoutExtension(itemSetFileName));
            fileIo.WriteAllText(itemSetFileName, itemSetJson);
            createdFiles.Add(itemSetFileName);

            DataTableCreator dataTableCreator = new();
            List<DataTable> dataTables = dataTableCreator.GetTables(allMatchesData.GetEntries());

            excelPrinter.PrintTablesToWorksheet(dataTables, statsFilePath);
            createdFiles.Add(statsFilePath);

            fileIo.WriteAllText(logFilePath, logger.GetContent());
            createdFiles.Add(logFilePath);

            logger.Log($"{createdFiles.Count} files written at {path}.");
            return createdFiles;
        }
    }
}