using System;
using CroquetScores.RavenDB.Documents;

namespace CroquetScores.RavenDBtoMySql.Competitions
{
    internal class TeamMatchProperties
    {
        public TeamMatchProperties(TeamMatch teamMatch)
        {
            Starts = teamMatch.Starts;
            Finishes = teamMatch.Finishes;
            ExpectedMatches = teamMatch.ExpectedMatches;
            BestOf = teamMatch.BestOf;
            Group = teamMatch.Group;
        }

        public DateTime Starts { get; private set; }
        public DateTime Finishes { get; private set; }
        public int ExpectedMatches { get; private set; }
        public int BestOf { get; private set; }
        public string Group { get; private set; }
    }
}