using System;
using System.Collections.Generic;

namespace Models.DBModels
{
    public partial class Maps
    {
        public Maps()
        {
            Matches = new HashSet<Matches>();
        }

        public string MapName { get; set; }

        public virtual ICollection<Matches> Matches { get; set; }
    }
}
