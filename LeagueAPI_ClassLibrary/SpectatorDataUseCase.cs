using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueAPI_ClassLibrary
{
    public class SpectatorDataUseCase
    {
        private readonly List<LeagueMatch> matches;
        private Dictionary<int, DamageDealt> champsAndDamage;

        public SpectatorDataUseCase(List<LeagueMatch> matches)
        {
            this.matches = matches ?? new List<LeagueMatch>();
            PopulateChampsAndDamage();
        }

        public string GetDamagePlayerIsPlayingAgainst(SpectatorData spectatorData, string playerId)
        {
            int playerTeamId = DeterminePlayerTeam(spectatorData, playerId);
            List<SpectatedParticipant> enemyTeam = GetEnemyPlayers(spectatorData, playerTeamId);
            DamageDealt dmg = GetDamageDealtFromEnemyTeam(enemyTeam);
            return dmg.ToString();
        }

        private DamageDealt GetDamageDealtFromEnemyTeam(List<SpectatedParticipant> enemyParticipants)
        {
            DamageDealt dmg = new(0, 0);
            foreach (SpectatedParticipant enemy in enemyParticipants)
            {
                int champId = enemy.championId.Value;
                dmg.IncrementDmg(champsAndDamage[champId]);
            }
            return dmg;
        }

        private static List<SpectatedParticipant> GetEnemyPlayers(SpectatorData spectatorData, int playerTeamId)
        {
            return spectatorData.participants.Where(
                p => p.teamId.Value != playerTeamId
            ).ToList();
        }

        private static int DeterminePlayerTeam(SpectatorData spectatorData, string playerId)
        {
            List<SpectatedParticipant> participants = spectatorData.participants;
            return participants.First(p => p.summonerId.Equals(playerId)).teamId.Value;
        }

        private void PopulateChampsAndDamage()
        {
            champsAndDamage = new Dictionary<int, DamageDealt>();
            AddMatchesData();
        }

        private void AddMatchesData()
        {
            foreach (LeagueMatch match in matches)
            {
                AddParticipantsData(match.participants);
            }
        }

        private void AddParticipantsData(List<Participant> participants)
        {
            foreach (Participant participant in participants)
            {
                AddParticipantData(participant);
            }
        }

        private void AddParticipantData(Participant participant)
        {
            int champId = participant.championId.GetValueOrDefault();
            int physDmg = participant.physicalDamageDealtToChampions;
            int magDmg = participant.magicDamageDealtToChampions;
            if (champsAndDamage.ContainsKey(champId))
            {
                champsAndDamage[champId].IncrementDmg(physDmg, magDmg);
            }
            else
            {
                champsAndDamage.Add(champId, new DamageDealt(physDmg, magDmg));
            }
        }

        private class DamageDealt
        {
            private long Physical { get; set; }
            private long Magical { get; set; }

            public DamageDealt(int phys, int mag)
            {
                Physical = phys;
                Magical = mag;
            }

            public void IncrementDmg(int phys, int mag)
            {
                Physical += phys;
                Magical += mag;
            }
            
            public void IncrementDmg(DamageDealt dmg)
            {
                Physical += dmg.Physical;
                Magical += dmg.Magical;
            }

            public override string ToString()
            {
                long allDmg = Physical + Magical;
                double percentPhys = GetPercent(allDmg, Physical);
                double percentMag = GetPercent(allDmg, Magical);
                return $"{percentPhys}/{percentMag} - {allDmg}";
            }

            private static double GetPercent(long allDmg, long dmg)
            {
                return Math.Round((double)dmg / allDmg * 100, 2);
            }
        }
    }
}