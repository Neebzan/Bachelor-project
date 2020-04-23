using Dapper;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    [Table("maps")]
    public class MapModel
    {
        [ExplicitKey]
        public string map_name { get; set; }
    }
}
