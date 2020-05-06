using System;
using System.Collections.Generic;

namespace DatabaseREST.Models
{
    public partial class PlayedMatch
    {
        public string PlayerId { get; set; }
        public uint MatchId { get; set; }
        public int? Score { get; set; }
        public int? Kills { get; set; }
        public int? Deaths { get; set; }

        public virtual Matches Match { get; set; }
        public virtual Players Player { get; set; }
    }
}
