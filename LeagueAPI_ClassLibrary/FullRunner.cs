using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class FullRunner
    {
        private IMatchSaver MatchSaver { get; set; }
        private ILeagueAPIClient LeagueApiClient { get; set; }
        private IMatchCollector MatchCollector { get; set; }
        private IDDragonRepository Repository { get; set; }
        private IDdragonRepositoryUpdater RepoUpdater { get; set; }
        private IFileIO FileIo { get; set; }
        private ILogger Logger { get; set; }

        public FullRunner(
            ILeagueAPIClient leagueApiClient,
            IMatchCollector matchCollector,
            IDDragonRepository repository,
            IFileIO fileIo,
            ILogger logger,
            IDdragonRepositoryUpdater repoUpdater, IMatchSaver matchSaver
        )
        {
            MatchCollector = matchCollector;
            Repository = repository;
            FileIo = fileIo;
            Logger = logger;
            RepoUpdater = repoUpdater;
            MatchSaver = matchSaver;
            LeagueApiClient = leagueApiClient;
        }

        public async Task<List<string>> DoFullRun(
            int queueId,
            string startPuuid,
            List<string> targetVersions,
            int maxCount,
            string existingMatchesFile,
            bool getLatestVersion,
            string outputDirectory
        )
        {
            try
            {
                List<LeagueMatch> alreadyScannedMatches = ReadExistingMatches(existingMatchesFile);

                Task updateTask = Task.CompletedTask;
                if (getLatestVersion) updateTask = RepoUpdater.GetLatestDdragonFiles();

                List<LeagueMatch> matches = 
                    await MatchCollector.GetMatches(startPuuid, queueId, targetVersions, maxCount, alreadyScannedMatches);
                await updateTask;
                Repository.RefreshData();
                return await MatchSaver.SaveMatches(matches);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                string fileName = Path.Combine(outputDirectory, "leagueApi_log.txt");
                FileIo.WriteAllText(fileName, Logger.GetContent());
                return new List<string>() {fileName};
            }
        }

        private List<LeagueMatch> ReadExistingMatches(string existingMatchesFile)
        {
            List<LeagueMatch> alreadyScannedMatches = null;
            bool matchesProvided = !existingMatchesFile.IsNullOrEmpty();
            if (matchesProvided)
            {
                Logger.Log("Reading already scanned matches...");
                alreadyScannedMatches = FileIo.ReadAllText(existingMatchesFile).DeserializeObject<List<LeagueMatch>>();
            }
            return alreadyScannedMatches;
        }
    }
}