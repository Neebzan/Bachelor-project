using System;
using System.Collections.Generic;

namespace Models.DBModels
{
    public partial class Wears
    {
        public string PlayerId { get; set; }
        public uint ItemId { get; set; }

        public virtual Items Item { get; set; }
        public virtual Players Player { get; set; }
    }
}
