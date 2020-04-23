using System;
using System.Collections.Generic;

namespace Models.DBModels
{
    public partial class Players
    {
        public Players()
        {
            HasLearned = new HashSet<HasLearned>();
            Items = new HashSet<Items>();
            PlayedMatch = new HashSet<PlayedMatch>();
            Wears = new HashSet<Wears>();
        }

        public string PlayerId { get; set; }
        public uint? Experience { get; set; }

        public virtual Accounts Player { get; set; }
        public virtual ICollection<HasLearned> HasLearned { get; set; }
        public virtual ICollection<Items> Items { get; set; }
        public virtual ICollection<PlayedMatch> PlayedMatch { get; set; }
        public virtual ICollection<Wears> Wears { get; set; }
    }
}
