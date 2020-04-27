using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using DatabaseREST.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace DatabaseREST.Controllers
{
    //This route makes this controller accessible at "localhost:5001/api/players
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly intrusiveContext _context;

        public PlayersController(intrusiveContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<Players> Get(string id, bool full)
        {
            Players player = null;

            if (full)
                player = _context.Players
                    .Include(p => p.HasLearned)
                        .ThenInclude(hl => hl.AbilityNameNavigation)
                    .Include(p => p.Wears)
                    .Include(p => p.Items)
                    .Include(p => p.PlayedMatch)
                    .SingleOrDefault(x => x.PlayerId == id);
            else
                player = _context.Players.Find(id);

            if (player == null)
                return NotFound();

            return player;
        }


        [HttpPut]
        public ActionResult Put(Players player)
        {
            var existingPlayer = _context.Abilities.Find(player.PlayerId);
            if (existingPlayer != null)
            {
                //Mark as existing and has been modified
                _context.Entry(player).State = EntityState.Modified;
                //Save
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("learn")]
        public ActionResult LearnAbility(string playerId, string abilityId)
        {
            //check if ability is already learned by player
            var temp = _context.Players.Where(p => p.PlayerId == playerId).Include(p => p.HasLearned).SingleOrDefault();
            if (temp.HasLearned.Any(a => a.AbilityName == abilityId))
                return Conflict();
            else
            {
                //Add the ability to the players HasLearned table
                temp.HasLearned.Add(new HasLearned()
                {
                    PlayerId = playerId,
                    AbilityName = abilityId
                });
                _context.SaveChanges();
                return Ok();
            }
        }

        [HttpPost]
        [Route("unlearn")]
        public ActionResult UnlearnAbility(string playerId, string abilityId)
        {
            //check if ability is already learned by player
            var tempPlayer = _context.Players.Where(p => p.PlayerId == playerId).Include(p => p.HasLearned).SingleOrDefault();
            var learned = tempPlayer.HasLearned.FirstOrDefault(a => a.AbilityName == abilityId);
            if (learned != null)
            {
                //Set the specific relation as deleted
                _context.Entry(learned).State = EntityState.Deleted;
                _context.SaveChanges();
                return Ok();
            }
            else
                return NotFound();
        }


    }
}