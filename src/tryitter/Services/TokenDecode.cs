using System.IdentityModel.Tokens.Jwt;

namespace tryitter.Services
{
    public class TokenDecode
    {
        public string GetUserIdFromToken(string idtoken)
        {
            var token = new JwtSecurityToken(jwtEncodedString: idtoken);
            string userId = token.Claims.First(c => c.Type == "UserLogin").Value;
            return userId;
        }
    }

}