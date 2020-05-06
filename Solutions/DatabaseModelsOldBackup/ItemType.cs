using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("item_types")]
    public class ItemType 
    {
        [ExplicitKey]
        public string type_value { get; set; }
    }
}
