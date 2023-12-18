using System;
using CroquetScores.RavenDB.Documents;

namespace CroquetScores.RavenDBtoMySql.Competitions
{
    internal class TeamMatchProperties
    {
        public TeamMatchProperties(TeamMatch teamMatch)
        {
            Start = teamMatch.Start;
            Finish = teamMatch.Finish;
            ExpectedMatches = teamMatch.ExpectedMatches;
            BestOf = teamMatch.BestOf;
            Group = teamMatch.Group;
        }

        public DateTime Start { get; private set; }
        public DateTime Finish { get; private set; }
        public int ExpectedMatches { get; private set; }
        public int BestOf { get; private set; }
        public string Group { get; private set; }
    }
}