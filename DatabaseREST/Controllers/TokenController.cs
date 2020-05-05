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
    public class TokenController : ControllerBase
    {
        private readonly intrusiveContext _context;
        private readonly intrusiveContextReadOnly _contextRead;

        public TokenController(intrusiveContext context, intrusiveContextReadOnly contextRead)
        {
            _context = context;
            _contextRead = contextRead;

        }



        [HttpPost]
        [Produces("application/json")]
        public ActionResult<TokenModel> NewAccesstoken([FromHeader]string token)
        {

            string newAccessToken = Token.NewAccessToken(token);

            if (newAccessToken != null)
            {
                TokenModel tokens = new TokenModel()
                {
                    AccessToken = newAccessToken
                };

                return tokens;
            }

            return Unauthorized("Refresh token expired or invalid!");
        }

        [HttpPost]
        [Route("server")]
        [Produces("application/json")]
        public ActionResult<string> NewServertoken([FromHeader]string serverSecret)
        {
            var server = _contextRead.Servers.Find(serverSecret);

            if(server != null)
            {
                return Token.GenerateServerToken();
            }

            return Unauthorized();
        }


    }
}