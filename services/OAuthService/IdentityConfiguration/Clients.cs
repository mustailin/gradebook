using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace OAuthService.IdentityConfiguration
{
    public class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
        {
            new Client
            {
                ClientId = "weatherApi",
                ClientName = "ASP.NET Core Weather Api",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = new List<Secret> {new Secret("ProCodeGuide".Sha256())},
                AllowedScopes = new List<string> {"weatherApi.read"}
            },
            new Client
            {
                ClientId = "gradebook-web",
                ClientName = "Gradebook Web App",
                ClientSecrets = new List<Secret> {new Secret("ProCodeGuide".Sha256())},

                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = new List<string> {"https://localhost:44346/signin-oidc"},
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "role",
                    "weatherApi.read",
                    "ROLE_USER",
                    "ROLE_ADMIN"
                },

                RequirePkce = true,
                AllowPlainTextPkce = false
            }
        };
        }
    }
}
