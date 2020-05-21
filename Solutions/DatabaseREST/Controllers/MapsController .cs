using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseREST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DatabaseREST.Controllers
{
    //This route makes this controller accessible at "localhost:5001/api/abilities
    [Route("api/[controller]")]
    [ApiController]
    public class MapsController : ControllerBase
    {
        private readonly intrusiveContext _context;

        public MapsController(intrusiveContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Maps>> Get()
        {
            return _context.Maps.ToList();
        }
    }
}