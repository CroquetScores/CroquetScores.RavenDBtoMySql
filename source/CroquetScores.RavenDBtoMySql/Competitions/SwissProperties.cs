using System;

namespace CroquetScores.RavenDBtoMySql.Competitions
{
    internal class SwissProperties
    {
        public SwissProperties(Guid[] carryForwardCompetitions)
        {
            CarryForwardCompetitions = carryForwardCompetitions;
        }

        public Guid[] CarryForwardCompetitions { get; private set; }
    }
}