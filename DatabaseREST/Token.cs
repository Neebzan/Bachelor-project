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

        private static string _secretKey = "SomeSecrffffffffffffffffffffffffffffffffffffffffffffffffffetKey";

        public static JwtSecurityToken GenerateToken(ClaimsIdentity claims, DateTime expirationDate)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            SigningCredentials sCredentials = new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);

            var token = tokenHandler.CreateJwtSecurityToken(
                expires: expirationDate,
                subject: claims,
                signingCredentials: sCredentials
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
        public static T GetModelFromToken<T>(string tokenString)
        {
            JwtSecurityToken token = new JwtSecurityToken(tokenString);
            return JsonConvert.DeserializeObject<T>(token.Claims.Where(c => c.Type == typeof(T).Name).Select(c => c.Value).FirstOrDefault().ToString());
        }

        public static Claim[] GetClaims(string tokenString)
        {
            JwtSecurityToken token = new JwtSecurityToken(tokenString);

            return token.Claims.ToArray();
        }

        public static string GetClaim(string key, string tokenString)
        {
            JwtSecurityToken token = new JwtSecurityToken(tokenString);

            return token.Claims.Where(c => c.Type == key).Select(c => c.Value).FirstOrDefault();
        }

        public static JwtSecurityToken GetTokenFromString(string tokenString)
        {
            return new JwtSecurityToken(tokenString);
        }

        public static string GenerateToken(string userID, string aud, DateTime expirationDate)
        {
            //Access token
            ClaimsIdentity accClaims = new ClaimsIdentity();
            accClaims.AddClaim(new Claim("sub", userID));
            accClaims.AddClaim(new Claim("aud", aud));

            return GenerateToken(accClaims, expirationDate).RawData;
        }

        public static string NewAccessToken(string refreshToken)
        {
            if(VerifyToken(refreshToken, "refresh"))
            {
                JwtSecurityToken tempToken = new JwtSecurityToken(refreshToken);

                string user = tempToken.Claims.Where(c => c.Type == "sub").Select(c => c.Value).FirstOrDefault();

                return GenerateToken(user, "access", DateTime.UtcNow.AddMinutes(15));
            }
            return null;
        }

        /// <summary>
        /// Check if the token is valid and has the correct signature
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool VerifyToken(string token, string aud = "")
        {
            if (token != null)
            {
                try
                {
                    //JwtSecurityToken tempToken = new JwtSecurityToken(token);
                    //if (aud != "")
                    //{
                    //    if (!tempToken.Audiences.Contains(aud))
                    //        return false;
                    //}

                    JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

                    TokenValidationParameters validationParameters = GetTokenValidationParameters(aud);
                    tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }


        private static SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symKey = Encoding.ASCII.GetBytes(_secretKey);
            return new SymmetricSecurityKey(symKey);
        }

        /// <summary>
        /// Get the Token Validation Parameters for the validation process
        /// </summary>
        /// <returns></returns>
        private static TokenValidationParameters GetTokenValidationParameters(string aud = "")
        {
            TokenValidationParameters validParams = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero, //Default clockskew is approx. 5 minutes
                IssuerSigningKey = GetSymmetricSecurityKey()
            };

            if(aud != "")
            {
                validParams.ValidateAudience = true;
                validParams.ValidAudience = aud;
            }

            return validParams;
        }
    }
}
