using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("items")]
    public class ItemModel 
    {
        [Key]
        public uint item_id { get; set; }
        public string item_color { get; set; }
        public string item_type { get; set; }
        public string owner_id { get; set; }
        public DateTime aquire_date { get; set; }
        public string item_name { get; set; }
        public float quality { get; set; }
    }
}
