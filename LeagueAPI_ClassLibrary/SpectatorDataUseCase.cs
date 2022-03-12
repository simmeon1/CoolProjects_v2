using System;
using System.Collections.Generic;
using System.Linq;

namespace LeagueAPI_ClassLibrary
{
    public class SpectatorDataUseCase
    {
        private Dictionary<int, DamageDealt> champsAndDamage;

        public SpectatorDataUseCase(List<LeagueMatch> matches)
        {
            PopulateChampsAndDamage(matches);
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
            double averagePhysical = 0;
            double averageMagical = 0;
            
            foreach (SpectatedParticipant enemy in enemyParticipants)
            {
                int champId = enemy.championId.Value;
                if (!champsAndDamage.ContainsKey(champId)) continue;
                
                DamageDealt entry = champsAndDamage[champId];
                averagePhysical += entry.GetAveragePhysical();
                averageMagical += entry.GetAverageMagical();
            }
            return new DamageDealt(averagePhysical, averageMagical);
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

        private void PopulateChampsAndDamage(List<LeagueMatch> matches)
        {
            champsAndDamage = new Dictionary<int, DamageDealt>();
            AddMatchesData(matches);
        }

        private void AddMatchesData(List<LeagueMatch> matches)
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
            DamageDealt dmg = new(participant.physicalDamageDealtToChampions, participant.magicDamageDealtToChampions);
            if (champsAndDamage.ContainsKey(champId))
            {
                champsAndDamage[champId].IncrementDmg(dmg);
            }
            else
            {
                champsAndDamage.Add(champId, dmg);
            }
        }

        private class DamageDealt
        {
            private double Physical { get; set; }
            private double Magical { get; set; }
            private int Entries { get; set; }

            public DamageDealt(double phys, double mag)
            {
                Physical = phys;
                Magical = mag;
                Entries = 1;
            }

            public void IncrementDmg(DamageDealt dmg)
            {
                Physical += dmg.Physical;
                Magical += dmg.Magical;
                Entries++;
            }
            
            public double GetAveragePhysical()
            {
                return Physical / Entries;
            }
            
            public double GetAverageMagical()
            {
                return Magical / Entries;
            }

            public override string ToString()
            {
                double allDmg = Physical + Magical;
                double percentPhys = GetPercent(allDmg, Physical);
                double percentMag = GetPercent(allDmg, Magical);
                return $"{percentPhys}/{percentMag} - {Round(allDmg)}";
            }

            private static double GetPercent(double allDmg, double dmg)
            {
                double value = dmg / allDmg * 100;
                return Round(value);
            }

            private static double Round(double value)
            {
                return Math.Round(value, 2);
            }
        }
    }
}