using System;
using System.Collections.Generic;
using CroquetScores.RavenDB.Documents.Types;

namespace CroquetScores.RavenDB.Documents
{
    public class Tournament : RavenDocument
    {
        public string Name { get; set; }

        public string Slug { get; set; }

        public DateTime Starts { get; set; }

        public DateTime Finishes { get; set; }

        public User.Reference CreatedBy { get; set; }

        public HashSet<User.Reference> Managers { get; set; }

        public HashSet<User.Reference> Scorers { get; set; }

        public List<Competition.Reference> Competitions { get; set; }

        public bool IsArchived { get; set; }

        public bool? IsMajorTournament { get; set; }

        public string SportType { get; set; }

        public string TournamentType { get; set; }

        public string TimeZoneInfo { get; set; }

        public class Reference : RavenDocumentReference
        {
            public string Name { get; set; }
        }
    }
}