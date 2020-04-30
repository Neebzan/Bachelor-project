using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseREST
{
    public static class Token
    {

        private static string _secretKey = "SomeSecretKey";

        private static JwtSecurityToken GenerateToken(ClaimsIdentity claims, int days = 0, int hours = 0, int minutes = 0)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            DateTime validDuration = DateTime.UtcNow;
            validDuration.AddDays(days);
            validDuration.AddHours(hours);
            validDuration.AddMinutes(minutes);

            var token = tokenHandler.CreateJwtSecurityToken(
                expires: validDuration,
                subject: claims,
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
                );

            return token;
        }

        //public static void Create()
        //{
        //    JwtPayload payload = new JWTPayload() { UserID = userModel.UserID, ServersInfo = data };
        //    string Token = JWTManager.CreateJWT(JWTManager.CreateClaims<JWTPayload>(payload), 5).RawData;
        //}

        /// <summary>
        /// Generate claims for a JWT token with a unique userID and a serializeable object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static ClaimsIdentity CreateClaims<T>(T model) where T : new()
        {
            ClaimsIdentity claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(model.GetType().Name, JsonConvert.SerializeObject(model)));

            return claims;
        }

        /// <summary>
        /// Gets the deserialized object from a JWT claim
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        public static T GetModelFromToken<T>(JwtSecurityToken token)
        {
            return JsonConvert.DeserializeObject<T>(token.Claims.Where(c => c.Type == typeof(T).Name).Select(c => c.Value).FirstOrDefault().ToString());
        }

        /// <summary>
        /// Gets the deserialized object from a JWT claim
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="token"></param>
        /// <returns></returns>
        public static T GetModelFromToken<T>(string tokenString)
        {
            JwtSecurityToken token = new JwtSecurityToken(tokenString);
            return JsonConvert.DeserializeObject<T>(token.Claims.Where(c => c.Type == typeof(T).Name).Select(c => c.Value).FirstOrDefault().ToString());
        }

        public static Claim[] GetClaims(string tokenString)
        {
            JwtSecurityToken token = new JwtSecurityToken(tokenString);

            return token.Claims.ToArray();

            //return JsonConvert.DeserializeObject<T>(token.Claims.Where(c => c.Type == typeof(T).Name).Select(c => c.Value).FirstOrDefault().ToString());
        }

        /// <summary>
        /// Check if the token is valid and has the correct signature
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool VerifyToken(string token)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            TokenValidationParameters validationParameters = GetTokenValidationParameters();
            try
            {
                ClaimsPrincipal tokenValid = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return false;
            }
        }


        private static SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symKey = Encoding.Default.GetBytes(_secretKey);
            return new SymmetricSecurityKey(symKey);
        }

        /// <summary>
        /// Get the Token Validation Parameters for the validation process
        /// </summary>
        /// <returns></returns>
        private static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = GetSymmetricSecurityKey()
            };
        }
    }
}
