using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("played_match")]
    public class PlayedMatch
    {
        [ExplicitKey]
        public string player_id { get; set; }
        [ExplicitKey]
        public int match_id { get; set; }
        public int score { get; set; }
        public int kills { get; set; }
        public int deaths { get; set; }
    }
}
