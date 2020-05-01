using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseREST.Models
{
    public class TokenModel
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }

    }
}
