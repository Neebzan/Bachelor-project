using System;
using System.Collections.Generic;

namespace DatabaseREST.Models
{
    public partial class Abilities
    {
        public Abilities()
        {
            HasLearned = new HashSet<HasLearned>();
        }

        public string AbilityName { get; set; }
        public int Cost { get; set; }

        public virtual ICollection<HasLearned> HasLearned { get; set; }
    }
}
