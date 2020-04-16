using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("has_learned")]
    public class UnlockedAbility
    {
        [ExplicitKey]
        public string player_id { get; set; }
        [ExplicitKey]
        public string ability_name { get; set; }
    }
}
