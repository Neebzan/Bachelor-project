using System;
using System.Collections.Generic;

namespace DatabaseREST.Models
{
    public partial class ItemColors
    {
        public ItemColors()
        {
            Items = new HashSet<Items>();
        }

        public string ColorName { get; set; }
        public float Red { get; set; }
        public float Green { get; set; }
        public float Blue { get; set; }

        public virtual ICollection<Items> Items { get; set; }
    }
}
