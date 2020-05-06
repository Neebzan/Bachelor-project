using System;
using System.Collections.Generic;

namespace DatabaseREST.Models
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

        private float LevelValue
        {
            get
            {
                return (float)(25 + Math.Sqrt(625 + 100 * (double)Experience)) / 50;
            }
        }

        public uint Level
        {
            get
            {
                var t = (uint)Math.Floor(LevelValue);
                return t;
            }
        }

        public float LevelProgress
        {
            get { return (LevelValue - Convert.ToSingle(Level)) * 100; }
        }

        public uint Skillpoints
        {
            get { return Level; }
        }
        public int SkillpointsUsed
        {
            get
            {
                int total = 0;
                foreach (var item in HasLearned)
                {
                    total += item.AbilityNameNavigation.Cost;
                }
                return total;
            }
        }


        public virtual Accounts Player { get; set; }
        public virtual ICollection<HasLearned> HasLearned { get; set; }
        public virtual ICollection<Items> Items { get; set; }
        public virtual ICollection<PlayedMatch> PlayedMatch { get; set; }
        public virtual ICollection<Wears> Wears { get; set; }
    }
}
