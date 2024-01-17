using System;
using CroquetScores.RavenDB.Documents.Types;

// ReSharper disable UnusedMember.Global

namespace CroquetScores.RavenDB.Documents
{
    public class TeamMatch : Competition
    {
        public string TeamA { get; set; }

        public string TeamB { get; set; }

        public DateTime Starts { get; set; }

        public DateTime Finishes { get; set; }

        public int ExpectedMatches { get; set; }

        public int BestOf { get; set; }

        public string Group { get; set; }
    }
}