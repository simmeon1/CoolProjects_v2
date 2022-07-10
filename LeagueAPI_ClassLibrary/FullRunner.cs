using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class FullRunner
    {
        private readonly IDDragonRepository repository;
        private readonly IDdragonRepositoryUpdater repoUpdater;
        private readonly IMatchCollector matchCollector;
        private readonly IMatchSaver matchSaver;
        private readonly IFileIO fileIo;
        private readonly ILogger logger;

        public FullRunner(
            IDDragonRepository repository,
            IFileIO fileIo,
            ILogger logger,
            IDdragonRepositoryUpdater repoUpdater,
            IMatchCollector matchCollector,
            IMatchSaver matchSaver
        )
        {
            this.repository = repository;
            this.fileIo = fileIo;
            this.logger = logger;
            this.repoUpdater = repoUpdater;
            this.matchCollector = matchCollector;
            this.matchSaver = matchSaver;
        }

        public async Task<List<string>> DoFullRun(Parameters p, List<string> parsedTargetVersions)
        {
            try
            {
                if (p.GetLatestDdragonData) await repoUpdater.GetLatestDdragonFiles();
                repository.RefreshData();

                List<LeagueMatch> alreadyScannedMatches = ReadExistingMatches(p.ExistingMatchesFile);
                string matchId = p.MatchId;
                if (Regex.IsMatch(matchId, @"^\d+$"))
                {
                    matchId = "EUW1_" + matchId;
                }
                
                List<LeagueMatch> matches =
                    await matchCollector.GetMatches(
                        p.AccountPuuid,
                        parsedTargetVersions,
                        p.MaxCount,
                        matchId,
                        alreadyScannedMatches
                    );
                return matchSaver.SaveMatches(matches);
            }
            catch (Exception ex)
            {
                logger.Log(ex.ToString());
                string fileName = Path.Combine(p.OutputDirectory, "leagueApi_log.txt");
                fileIo.WriteAllText(fileName, logger.GetContent());
                logger.Log($"Log file written at {p.OutputDirectory}.");
                return new List<string> {fileName};
            }
        }

        private List<LeagueMatch> ReadExistingMatches(string existingMatchesFile)
        {
            bool matchesProvided = !existingMatchesFile.IsNullOrEmpty();
            if (!matchesProvided) return null;

            logger.Log("Reading already scanned matches...");
            return fileIo.ReadAllText(existingMatchesFile).DeserializeObject<List<LeagueMatch>>();
        }
    }
}