using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("item_colors")]
    public class ItemColor 
    {
        [ExplicitKey]
        public string color_name { get; set; }
        public float red { get; set; }
        public float green { get; set; }
        public float blue { get; set; }

    }
}
