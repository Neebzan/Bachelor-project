using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseREST.Models
{
    public class HighscoreEntry
    {
        public int Rank { get; set; }
        public Players Player { get; set; }
    }
}
