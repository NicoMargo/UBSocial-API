using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace UbSocial.Models.Helpers
{
    public class JWT
    {
        public static string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9FKIW7hfFlkfdsfjglkjfasdf"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //Create claims
            var claims = new[]
            {
                new Claim(ClaimTypes.Sid, Convert.ToString(user.Id))
            };

            //Create token
            var token = new JwtSecurityToken(
                "api.UBSocial.com",
                "User",
                claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }
    }
}
