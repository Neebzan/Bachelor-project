using System;
using System.Collections.Generic;

namespace DatabaseREST.Models
{
    public partial class ItemTypes
    {
        public ItemTypes()
        {
            Items = new HashSet<Items>();
        }

        public string TypeValue { get; set; }

        public virtual ICollection<Items> Items { get; set; }
    }
}
