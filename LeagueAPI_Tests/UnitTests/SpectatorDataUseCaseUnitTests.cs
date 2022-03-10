using Common_ClassLibrary;
using LeagueAPI_ClassLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeagueAPI_Tests.UnitTests
{
    [TestClass]
    public class SpectatorDataUseCaseUnitTests
    {
        [TestMethod]
        public void GetsExpectedResults()
        {
            List<Participant> participants1 = new() {GetParticipant(2, 10, 90)};
            List<Participant> participants2 = new() {GetParticipant(2, 40, 110)};


            List<LeagueMatch> matches = new() {GetMatch(participants1), GetMatch(participants2),};

            List<SpectatedParticipant> spectatedParticipants = new()
            {
                GetSpectatedParticipant(1, "me", 1),
                GetSpectatedParticipant(2, "enemy", 2),
            };

            SpectatorData specData = new(spectatedParticipants);
            SpectatorDataUseCase useCase = new(matches);
            string result = useCase.GetDamagePlayerIsPlayingAgainst(specData, "me");
            Assert.IsTrue(result.Equals("20/80 - 250"));
        }

        private static LeagueMatch GetMatch(List<Participant> participants1)
        {
            return new LeagueMatch()
            {
                participants = participants1
            };
        }

        private static Participant GetParticipant(int championId, int physicalDamageDealtToChampions,
            int magicDamageDealtToChampions)
        {
            return new Participant
            {
                championId = championId,
                physicalDamageDealtToChampions = physicalDamageDealtToChampions,
                magicDamageDealtToChampions = magicDamageDealtToChampions
            };
        }

        private static SpectatedParticipant GetSpectatedParticipant(int championId, string summonerId, int teamId)
        {
            return new SpectatedParticipant()
            {
                championId = championId,
                summonerId = summonerId,
                teamId = teamId
            };
        }
    }
}