using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("wears")]
    public class Wears
    {
        [ExplicitKey]
        public string player_id { get; set; }
        [ExplicitKey]
        public uint item_id { get; set; }
    }
}
