using GradeBook.Data.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace GradeBook.Common
{
    public class JwtHelper
    {
        private const string UserIdClaim = "userId";
        private const string RoleClaim = "role";

        public static UserViewModel GetUserInfo(HttpRequest request)
        {
            if (!request.Headers.TryGetValue("authorization", out StringValues value))
                return null;

            var jwt = value.ToString().Split(' ').Last();
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var claims = token.Claims.ToList();
            int.TryParse(claims.FirstOrDefault(c => c.Type == UserIdClaim)?.Value, out int userId);

            return new UserViewModel
            {
                Id = userId,
                Email = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value,
                FirstName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value,
                LastName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value,
                Role = claims.FirstOrDefault(c => c.Type == RoleClaim)?.Value,
            };
        }

        public static string GenerateJSONWebToken(UserViewModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SecretGradeBook!"));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(UserIdClaim, userInfo.Id.ToString()),
                new Claim(RoleClaim, userInfo.Role),
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, userInfo.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, userInfo.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                "GradeBook",
                "GradeBook",
                claims,
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
