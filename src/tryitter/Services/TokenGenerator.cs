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
        public string Generate(UserDTO user)
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
            return tokenHandler.WriteToken(token);
        }

        private ClaimsIdentity AddClaims(UserDTO user)
        {
            var claims = new ClaimsIdentity();

            claims.AddClaim(new Claim("Adm", user.Email.ToString()));

            // if (client.IsCompany)
            //     claims.AddClaim(new Claim("IsCompany", ClientTypeEnum.PessoaJuridica.ToString()));
            // else
            //     claims.AddClaim(new Claim("IsCompany", ClientTypeEnum.PessoaFisica.ToString()));

            return claims;
        }
    }
}

