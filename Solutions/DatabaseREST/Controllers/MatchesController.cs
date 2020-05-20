﻿using System;
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
    public class MatchesController : ControllerBase
    {
        private readonly intrusiveContext _context;

        public MatchesController(intrusiveContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Matches>> Get()
        {
            return _context.Matches.OrderBy(u => u.Ended).Take(1000).ToList();
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<Matches> Get(string id)
        {

            var match = _context.Matches.Find(id);
            if (match == null)
                return NotFound();

            return match;
        }

        public ActionResult<Matches> Post(Matches match)
        {
            //Check if match exists
            var existingMatch = _context.Matches.Find(match.MatchId);
            if (existingMatch == null)
            {
                try
                {
                    _context.Matches.Add(match);                        
                    _context.SaveChanges();
                }
                catch (DbUpdateException e)
                {
                    ModelState.AddModelError("Exception", e.InnerException.Message);
                    return StatusCode(500, ModelState);
                }
                return Created(string.Empty, match);
            }
            else
            {
                ModelState.AddModelError("MatchId", "A match with that ID already exists!");
                return Conflict(ModelState);
            }

        }
    }
}