using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseREST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

namespace DatabaseREST.Controllers
{
    //This route makes this controller accessible at "localhost:5001/api/commands
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly intrusiveContext _context;
        private readonly intrusiveContextReadOnly _contextRead;

        public TestController(intrusiveContext context, intrusiveContextReadOnly contextRead)
        {
            _context = context;
            _contextRead = contextRead;
        }

        public string[] GetHostName(string connectionString)
        {
            string variableName = "";
            string value = "";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SHOW VARIABLES WHERE Variable_name = 'hostname'";

                var cmd = new MySqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    variableName = reader.GetString(0);
                    value = reader.GetString(1);
                    Console.WriteLine("{0} + {1}", variableName, value);

                }
            }
            return new string[] { variableName, value };
        }

        [HttpGet]
        [Route("master")]
        public ActionResult<string> TestHostNameMaster()
        {
            return GetHostName(_context.Database.GetDbConnection().ConnectionString)[1] + " - " + _context.Database.GetDbConnection().ConnectionString;
        }

        [HttpGet]
        [Route("slave")]
        public ActionResult<string> TestHostNameSlave()
        {
            return GetHostName(_contextRead.Database.GetDbConnection().ConnectionString)[1] +" - "+ _contextRead.Database.GetDbConnection().ConnectionString;
        }

        [HttpGet]
        public ActionResult<IEnumerable<string>> GetString()
        {
            return new List<string>()
            {
                "This", "is", "some", "strings"
            };
        }
    }
}