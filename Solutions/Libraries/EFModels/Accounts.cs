using System;
using System.Collections.Generic;

namespace DatabaseREST.Models
{
    public partial class Accounts
    {
        public string AccountId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }

        public virtual Players Players { get; set; }
    }
}
