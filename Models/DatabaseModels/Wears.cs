using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("wears")]
    public class Wears
    {
        [Key]
        public string player_id { get; set; }
        [Key]
        public string item_type { get; set; }
    }
}
