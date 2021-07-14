using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class MatchCollector
    {
        private ILeagueAPIClient Client { get; set; }
        public MatchCollector(ILeagueAPIClient client)
        {
            Client = client;
        }

        /// <summary>
        /// Gets a result saying if the target version is greater than the game version.
        /// </summary>
        /// <param name="targetVersion"></param>
        /// <param name="gameVersion"></param>
        /// <returns>1 if target is greater than game version, 0 if equal, -1 if lesser.</returns>
        public int CompareTargetVersionAgainstGameVersion(string targetVersion, string gameVersion)
        {
            string[] targetVersionArray = targetVersion.Split('.');
            string[] gameVersionArray = gameVersion.Split('.');

            int maxLength = Math.Max(targetVersionArray.Length, gameVersionArray.Length);
            for (int i = 0; i < maxLength; i++)
            {
                int targetVersionCharDigit = i > targetVersionArray.Length - 1 ? 0 : int.Parse(targetVersionArray[i].ToString());
                int gameVersionCharDigit = i > gameVersionArray.Length - 1 ? 0 : int.Parse(gameVersionArray[i].ToString());
                if (targetVersionCharDigit != gameVersionCharDigit) return targetVersionCharDigit > gameVersionCharDigit ? 1 : -1;
            }
            return 0;
        }
    }
}
