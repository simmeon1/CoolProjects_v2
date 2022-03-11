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
            List<Participant> participants1 = new()
            {
                GetParticipant(1, 10, 90),
                GetParticipant(2, 20, 80),
                GetParticipant(3, 30, 70),
                GetParticipant(4, 40, 60),
            };
            List<Participant> participants2 = new()
            {
                GetParticipant(3, 50, 50),
                GetParticipant(4, 60, 40),
                GetParticipant(5, 70, 30),
                GetParticipant(6, 80, 20),
            };


            List<LeagueMatch> matches = new() {GetMatch(participants1), GetMatch(participants2)};

            List<SpectatedParticipant> spectatedParticipants = new()
            {
                GetSpectatedParticipant(1, "me", 1),
                GetSpectatedParticipant(2, "ally", 1),
                GetSpectatedParticipant(3, "enemy1", 2),
                GetSpectatedParticipant(4, "enemy2", 2),
            };

            SpectatorData specData = new(spectatedParticipants);
            SpectatorDataUseCase useCase = new(matches);
            string result = useCase.GetDamagePlayerIsPlayingAgainst(specData, "me");
            Assert.IsTrue(result.Equals("45/55 - 400"));
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