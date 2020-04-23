using System;
using System.Collections.Generic;

namespace DatabaseREST.Models
{
    public partial class Matches
    {
        public Matches()
        {
            PlayedMatch = new HashSet<PlayedMatch>();
        }

        public uint MatchId { get; set; }
        public string MapName { get; set; }
        public DateTime Begun { get; set; }
        public DateTime Ended { get; set; }
        public int Difficulty { get; set; }

        public virtual Maps MapNameNavigation { get; set; }
        public virtual ICollection<PlayedMatch> PlayedMatch { get; set; }
    }
}
