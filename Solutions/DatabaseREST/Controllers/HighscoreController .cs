using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
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
        public ActionResult<HighscorePage> Get([FromHeader]string token, int perPage = 10, int page = 1, string id = "")
        {
            if (Token.VerifyToken(token, "access"))
            {
                int rankIndex = 1;
                int curPage = 1;
                List<Players> players;

                HighscorePage hPage = new HighscorePage()
                {
                    CurrentPage = 1,
                    TotalPageCount = 1
                };

                if (id != "")
                {
                    Players player = _contextRead.Players.Find(id);
                    if (player != null)
                    {
                        //Minus one to always get the correct page number based on
                        //page 0 = 1..10
                        //page 1 = 11..20 etc.
                        curPage = (_context.Players.Where(p => p.Experience >= player.Experience).Count() - 1) / perPage;



                        players = _contextRead.Players.OrderByDescending(p => p.Experience).Skip(perPage * curPage).Take(perPage).ToList();

                        rankIndex = curPage * perPage + 1;

                    }
                    else
                        return NotFound();
                }
                else
                {
                    curPage = page;
                    if (curPage < 1)
                    {
                        curPage = 1;
                    }
                    players = _contextRead.Players.OrderByDescending(p => p.Experience).Skip(perPage * (curPage - 1)).Take(perPage).ToList();
                    rankIndex = (curPage - 1) * perPage + 1;
                }

                List<HighscoreEntry> result = new List<HighscoreEntry>();
                //Plus one to show correct rank based on 1 indexed ranks

                for (int i = 0; i < players.Count; i++)
                {
                    result.Add(new HighscoreEntry()
                    {
                        Player = players[i],
                        Rank = rankIndex
                    });
                    rankIndex++;
                }

                hPage.CurrentPage = curPage;
                hPage.Entries = result;
                hPage.TotalPageCount = (_contextRead.Players.Count() / perPage) + 1;

                return hPage;

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