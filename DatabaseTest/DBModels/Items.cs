using System;
using System.Collections.Generic;

namespace Models.DBModels
{
    public partial class Items
    {
        public Items()
        {
            Wears = new HashSet<Wears>();
        }

        public uint ItemId { get; set; }
        public string ItemColor { get; set; }
        public string ItemType { get; set; }
        public string OwnerId { get; set; }
        public DateTime AquireDate { get; set; }
        public string ItemName { get; set; }
        public float Quality { get; set; }

        public virtual ItemColors ItemColorNavigation { get; set; }
        public virtual ItemTypes ItemTypeNavigation { get; set; }
        public virtual Players Owner { get; set; }
        public virtual ICollection<Wears> Wears { get; set; }
    }
}
