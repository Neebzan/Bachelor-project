using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseREST.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseREST.Controllers
{
    //This route makes this controller accessible at "localhost:5001/api/database
    [Route("api/[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {
        private readonly intrusiveContext _context;

        public DatabaseController(intrusiveContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Accounts>> GetAccounts()
        {
            //var accounts = _context.Accounts.OrderByDescending(u => u.AccountId).Take(10);
            //return accounts.ToList();
            //LOLOLOL
            return _context.Accounts;
        }

        //[HttpGet]
        //public ActionResult<IEnumerable<string>> GetAccount()
        //{
        //    return new List<string>()
        //    {
        //        "This", "is", "some", "database"
        //    };
        //}
    }
}