using IdentityServer4.Models;
using System.Collections.Generic;

namespace OAuthService.IdentityConfiguration
{
    public class Scopes
    {
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("weatherApi.read", "Read Access to Weather API"),
                new ApiScope("weatherApi.write", "Write Access to Weather API"),
                new ApiScope("ROLE_USER", "User Role for GradeBook"),
                new ApiScope("ROLE_ADMIN", "Admin Role for GradeBook")
            };
        }
    }
}
