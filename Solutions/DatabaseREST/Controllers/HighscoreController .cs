using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DatabaseREST.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace DatabaseREST.Controllers
{
    //This route makes this controller accessible at "localhost:5001/api/database
    [Route("api/[controller]")]
    [ApiController]
    public class HighscoreController : ControllerBase
    {
        private readonly intrusiveContext _context;
        private readonly intrusiveContextReadOnly _contextRead;

        public HighscoreController(intrusiveContext context, intrusiveContextReadOnly contextRead)
        {
            _context = context;
            _contextRead = contextRead;

        }

        [HttpGet]
        [Route("xp")]
        public ActionResult<IEnumerable<HighscoreEntry>> Get([FromHeader]string token, int perPage = 10, int page = 0, string id = "")
        {
            if (Token.VerifyToken(token, "access"))
            {
                int rankIndex = 1;
                List<Players> players;

                if (id != "")
                {
                    Players player = _contextRead.Players.Find(id);
                    if (player != null)
                    {
                        //Minus one to always get the correct page number based on
                        //page 0 = 1..10
                        //page 1 = 11..20 etc.
                        int curPage = (_context.Players.Where(p => p.Experience >= player.Experience).Count() - 1) / perPage;

                        players = _contextRead.Players.OrderByDescending(p => p.Experience).Skip(perPage * curPage).Take(perPage).ToList();

                        //Plus one to show correct rank based on 1 indexed ranks
                        rankIndex = curPage * perPage + 1;

                    }
                    else
                        return NotFound();
                }
                else
                    players = _contextRead.Players.OrderByDescending(p => p.Experience).Skip(perPage * page).Take(perPage).ToList();

                List<HighscoreEntry> result = new List<HighscoreEntry>();
                for (int i = 0; i < players.Count; i++)
                {
                    result.Add(new HighscoreEntry()
                    {
                        Player = players[i],
                        Rank = rankIndex
                    });
                    rankIndex++;
                }

                return result;

            }
            return Unauthorized("Token invalid or expired!");
        }

        [HttpGet]
        [Route("xp/player")]
        public ActionResult<HighscoreEntry> PlayerHighscore(string id, [FromHeader]string token)
        {
            if (Token.VerifyToken(token, "access"))
            {
                HighscoreEntry result = new HighscoreEntry();

                result.Player = _contextRead.Players.Find(id);
                if (result.Player != null)
                {
                    result.Rank = _context.Players.Where(p => p.Experience >= result.Player.Experience).Count();
                    return result;
                }
                return NotFound();

            }
            return Unauthorized("Token invalid or expired!");
        }

    }
}