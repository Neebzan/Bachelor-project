﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseREST.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySql.Data.MySqlClient;

namespace DatabaseREST.Controllers
{
    //This route makes this controller accessible at "localhost:5001/api/database
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly intrusiveContext _context;
        private readonly intrusiveContextReadOnly _contextRead;

        public AccountsController(intrusiveContext context, intrusiveContextReadOnly contextRead)
        {
            _context = context;
            _contextRead = contextRead;
            
        }

        //[HttpGet]
        //public ActionResult<IEnumerable<Accounts>> Get()
        //{
        //    return _context.Accounts.OrderBy(u => u.AccountId).Take(1000).ToList();
        //}

       

        [HttpGet]
        public ActionResult<Accounts> Get(string id)
        {
            //var t = Request.Host;
            //Console.WriteLine("Host test: "+ t);
            var acc = _contextRead.Accounts.Find(id);
            if (acc == null)
                return NotFound();

            return acc;
        }

        [HttpPost]
        public ActionResult<Accounts> PostAccount(Accounts acc)
        {
            //Check if account exists
            var existingAccId = _context.Accounts.Find(acc.AccountId);
            if (existingAccId == null)
            {
                var existingAccEmail = _context.Accounts.Where(a => a.Email == acc.Email).FirstOrDefault();
                if (existingAccEmail == null)
                {

                    try
                    {
                        _context.Accounts.Add(acc);
                        //Also add a player for the account
                        _context.Players.Add(
                            new Players()
                            {
                                PlayerId = acc.AccountId
                            });
                        _context.SaveChanges();
                    }
                    catch (DbUpdateException e)
                    {
                        ModelState.AddModelError("Exception", e.InnerException.Message);
                        return StatusCode(500, ModelState);
                    }
                    return Created(string.Empty, acc);
                }
                ModelState.AddModelError("Email", "Email already in use!");
                return Conflict(ModelState);
            }
            ModelState.AddModelError("AccountId", "An account with that name already exists!");
            return Conflict(ModelState);
        }

        [HttpPut]
        public ActionResult PutAccount(Accounts acc)
        {
            var existingAcc = _context.Accounts.Find(acc.AccountId);
            if (existingAcc != null && existingAcc.PasswordHash == acc.PasswordHash)
            {
                //Mark as existing and has been modified
                _context.Entry(acc).State = EntityState.Modified;
                //Save
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
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