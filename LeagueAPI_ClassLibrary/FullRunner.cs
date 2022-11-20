using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class FullRunner
    {
        private readonly IDDragonRepository repository;
        private readonly IMatchCollector matchCollector;
        private readonly IMatchSaver matchSaver;
        private readonly ILeagueAPIClient client;
        private readonly IFileIO fileIo;
        private readonly ILogger logger;

        public FullRunner(
            IDDragonRepository repository,
            IFileIO fileIo,
            ILogger logger,
            IMatchCollector matchCollector,
            IMatchSaver matchSaver,
            ILeagueAPIClient client
        )
        {
            this.repository = repository;
            this.fileIo = fileIo;
            this.logger = logger;
            this.matchCollector = matchCollector;
            this.matchSaver = matchSaver;
            this.client = client;
        }

        public async Task<List<string>> DoFullRun(Parameters p)
        {
            try
            {
                List<string> versionsJson = await client.GetLatestVersions();
                List<string> parsedTargetVersions = GetParsedListOfVersions(p.RangeOfTargetVersions, versionsJson);
                string versionsStr = parsedTargetVersions.ConcatenateListOfStringsToCommaString();
                string latestVersion = parsedTargetVersions.FirstOrDefault();
                await repository.RefreshData(latestVersion);
                matchSaver.SetOutputDetails(p.OutputDirectory, versionsStr);

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

        private List<string> GetParsedListOfVersions(List<string> unparsedVersions, List<string> versionsJson)
        {
            List<string> parsedVersions = new();
            foreach (string unparsedVersion in unparsedVersions)
            {
                int unparsedVersionIndex = int.Parse(unparsedVersion);
                int unparsedVersionUpdatedIndex = unparsedVersionIndex * -1;
                string parsedVersion = versionsJson[unparsedVersionUpdatedIndex];
                parsedVersions.Add(parsedVersion);
            }

            return parsedVersions;
        }
    }
}