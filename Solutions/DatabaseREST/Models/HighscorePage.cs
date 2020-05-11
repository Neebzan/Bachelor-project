using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseREST.Models
{
    public class HighscorePage
    {
        public int CurrentPage { get; set; }
        public int TotalPageCount { get; set; }
        public List<HighscoreEntry> Entries { get; set; }
    }
}
