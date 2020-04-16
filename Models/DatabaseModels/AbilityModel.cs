using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("abilities")]
    public class AbilityModel
    {
        [ExplicitKey]
        public string ability_name { get; set; }
        public int cost{ get; set; }
    }
}
