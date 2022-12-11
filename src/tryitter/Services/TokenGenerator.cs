using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using tryitter.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace tryitter.Services
{
    public class TokenGenerator
    {
        public Token Generate(UserDTO user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = AddClaims(user),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes("SecretSecretSecretSecret")), // ---> Colocar secret em uma váriavel de ambiente
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Expires = DateTime.Now.AddDays(5)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenGenerated = tokenHandler.WriteToken(token);
            var tokenObject = new Token { TokenMessage = tokenGenerated };

            return tokenObject;
        }

        static ClaimsIdentity AddClaims(UserDTO user)
        {
            var claims = new ClaimsIdentity();

            if (user.Email == "adm@email.com")
            {
                claims.AddClaim(new Claim("AdmLogin", user.Email.ToString()));
            }
            else
            {
                claims.AddClaim(new Claim("UserLogin", user.UserId.ToString()));
            }

            return claims;
        }
    }
}

