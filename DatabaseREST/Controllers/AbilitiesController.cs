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
    public class AbilitiesController : ControllerBase
    {
        private readonly intrusiveContext _context;

        public AbilitiesController(intrusiveContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Abilities>> Get()
        {
            return _context.Abilities.OrderBy(u => u.AbilityName).Take(1000).ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Abilities> Get(string id)
        {

            var abb = _context.Abilities.Find(id);
            if (abb == null)
                return NotFound();

            return abb;
        }

        public ActionResult<Abilities> Post(Abilities abb)
        {
            //Check if account exists
            var existingAbbId = _context.Accounts.Find(abb.AbilityName);
            if (existingAbbId == null)
            {
                try
                {
                    _context.Abilities.Add(abb);                        
                    _context.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    ModelState.AddModelError("Exception", e.InnerException.Message);
                    return StatusCode(500, ModelState);
                }
                return Created(string.Empty, abb);
            }
            else
            {
                ModelState.AddModelError("AbilityName", "An ability with that name already exists!");
                return Conflict(ModelState);
            }

        }

        [HttpPut]
        public ActionResult Put(Abilities abb)
        {
            var existingAbb = _context.Abilities.Find(abb.AbilityName);
            if (existingAbb != null)
            {
                //Mark as existing and has been modified
                _context.Entry(abb).State = EntityState.Modified;
                //Save
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }
    }
}