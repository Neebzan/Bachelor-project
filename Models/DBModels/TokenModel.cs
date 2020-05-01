using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseREST.DBModels
{
    public class TokenModel
    {
        public string RefreshToken { get; set; }
        public string AccessToken { get; set; }

    }
}
