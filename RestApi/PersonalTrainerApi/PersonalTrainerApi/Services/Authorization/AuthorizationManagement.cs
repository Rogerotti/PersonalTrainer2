using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace PersonalTrainerApi.Services.Authorization
{
    public class AuthorizationManagement : IAuthorizationManagement
    {
        public string GenerateToken(bool isAdmin = false)
        {
            // Define const Key this should be private secret key stored in some safe place
            string key = "401b09eab3c013d4ca54922bb802bec8fd5318165da43fd1d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";

            // Create Security key  using private key above:
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            var credentials = new SigningCredentials
                              (securityKey, SecurityAlgorithms.HmacSha256Signature);

            //  Finally create a Token
            var header = new JwtHeader(credentials);
            //Some PayLoad that contain information about the  customer
            JwtPayload payload;
            if (isAdmin)
            {
                payload = new JwtPayload
                   {
                       {"userType:user", "user"},
                       {"userType:admin", "admin" },
                       {"expireDate", DateTime.Now.AddMinutes(30).ToString() },
                       {"scope", "https://personal-trainer.com/"},
                   };
            }
            else
            {
                payload = new JwtPayload
                   {
                       {"userType:user", "user"},
                       {"expireDate", DateTime.Now.AddMinutes(30).ToString() },
                       {"scope", "https://personal-trainer.com/"},
                   };
            }

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();
            var tokenString = handler.WriteToken(secToken);
            return tokenString;
        }
    }
}
