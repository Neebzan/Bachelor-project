using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DatabaseREST.Controllers
{
    //This route makes this controller accessible at "localhost:5001/api/commands
    [Route("api/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
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