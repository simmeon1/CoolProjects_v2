using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;

namespace LeagueApiSpectator_Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logger_Console logger = new();
            
            logger.Log("Reading parameters file...");
            
            string parametersPath = "";
            foreach (string arg in args)
            {
                Match match = Regex.Match(arg, "parametersPath-(.*)");
                if (match.Success) parametersPath = match.Groups[1].Value;
            }
            Parameters parameters = DeserializeJsonFile<Parameters>(parametersPath);

            logger.Log("Reading matches file...");
            List<LeagueMatch> matches = DeserializeJsonFile<List<LeagueMatch>>(parameters.MatchesPath);

            logger.Log("Creating objects...");
            LeagueAPIClient leagueClient = new(
                new RealHttpClient(),
                parameters.Token,
                new RealDelayer(),
                logger
            );
            
            string encryptedSummonerId = parameters.Id;
            SpectatorDataUseCase useCase = new(matches);

            while (true)
            {
                logger.Log("Retrieving spectator data...");
                SpectatorData spectatorData = await leagueClient.GetSpectatorDataByEncryptedSummonerId(encryptedSummonerId);
                logger.Log(spectatorData == null
                    ? "User is not in game."
                    : useCase.GetDamagePlayerIsPlayingAgainst(spectatorData, encryptedSummonerId));
                logger.Log("Press any key to repeat process.");
                logger.ReadKey();
            }
        }

        private static T DeserializeJsonFile<T>(string parametersPath)
        {
            return File.ReadAllText(parametersPath).DeserializeObject<T>();
        }
    }

    public class Parameters
    {
        public string Id { get; set; }
        public string Token { get; set; }
        public string MatchesPath { get; set; }
    }
}