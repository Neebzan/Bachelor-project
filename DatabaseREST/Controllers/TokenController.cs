using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DatabaseREST.DBModels;
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
    public class TokenController : ControllerBase
    {
        private readonly intrusiveContext _context;
        private readonly intrusiveContextReadOnly _contextRead;

        public TokenController(intrusiveContext context, intrusiveContextReadOnly contextRead)
        {
            _context = context;
            _contextRead = contextRead;
            
        }       

        //[HttpPost]
        //[Route("refresh")]
        //public ActionResult<string> Login(string refreshToken)
        //{

        //    var accTemp = _contextRead.Accounts.Find(acc.AccountId);
        //    if (accTemp == null)
        //    {
        //        return NoContent();
        //    }

        //    if (acc.PasswordHash == accTemp.PasswordHash)
        //    {
        //        //Refresh token
        //        ClaimsIdentity refClaims = new ClaimsIdentity();
        //        refClaims.AddClaim(new Claim("sub", acc.AccountId));
        //        refClaims.AddClaim(new Claim("aud", "refresh"));

        //        string refToken = Token.GenerateToken(refClaims, 14, 0, 0).RawData;
                
        //        //Access token
        //        ClaimsIdentity accClaims = new ClaimsIdentity();
        //        accClaims.AddClaim(new Claim("sub", acc.AccountId));
        //        accClaims.AddClaim(new Claim("aud", "access"));

        //        string accToken = Token.GenerateToken(accClaims, 0, 0, 10).RawData;

        //        TokenModel tokens = new TokenModel()
        //        {
        //            AccessToken = accToken,
        //            RefreshToken = refToken
        //        };

        //        return JsonConvert.SerializeObject(tokens);
        //    }
            
        //    return Unauthorized("Password did not match!");

        //}
    }
}