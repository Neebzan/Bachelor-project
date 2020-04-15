using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("matches")]
    public class MatchModel
    {
        [Key]
        public int match_id { get; set; }
        public string map_name { get; set; }
        public DateTime begun { get; set; }
        public DateTime ended { get; set; }
        public int difficulty { get; set; }

    }
}
