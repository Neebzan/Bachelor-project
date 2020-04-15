using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("players")]
    public class PlayerModel 
    {
        [ExplicitKey]
        public string player_id { get; set; }
        public int experience { get; set; }
    }
}
