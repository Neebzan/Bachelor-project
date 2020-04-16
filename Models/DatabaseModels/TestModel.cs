using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class TestModel
    {
        public uint match_id { get; set; }
        public string map_name { get; set; }
        public DateTime begun { get; set; }
        public DateTime ended { get; set; }
        public int difficulty { get; set; }
        public int score { get; set; }
        public int kills { get; set; }
        public int deaths { get; set; }
    }
}
