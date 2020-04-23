using System;
using System.Collections.Generic;

namespace Models.DBModels
{
    public partial class HasLearned
    {
        public string PlayerId { get; set; }
        public string AbilityName { get; set; }

        public virtual Abilities AbilityNameNavigation { get; set; }
        public virtual Players Player { get; set; }
    }
}
