using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;

namespace Models
{
    [Table("accounts")]
    public class AccountModel
    {
        [ExplicitKey]
        public string account_id { get; set; }
        public string email { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string password_hash { get; set; }

        //public DatabaseResponse Status;
    }
}
