using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace PersonalTrainerApi.Services
{
    public class AuthorizationManagement : IAuthorizationManagement
    {
        public string GenerateToken(string username)
        {
                var now = DateTime.UtcNow;

                // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
                // You can add other claims here, if you want:
                var dateTimeOffset = new DateTimeOffset(now);
                var unixDateTime = dateTimeOffset.ToUnixTimeSeconds();

                var claims = new Claim[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, unixDateTime.ToString(), ClaimValueTypes.Integer64)
                };

                // Create the JWT and write it to a string
                var jwt = new JwtSecurityToken(
                    claims: claims,
                    notBefore: now,
                    expires: now.Add(TimeSpan.FromMinutes(30))
                    );
                return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        public bool TokenValid(string token)
        {
            var result = new JwtSecurityTokenHandler().ReadToken(token);
            return result.ValidTo < DateTime.Now;

        }
    }
}
