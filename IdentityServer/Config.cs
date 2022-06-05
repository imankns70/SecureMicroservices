using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityServer
{
    public class Config
    {
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                   {
                       ClientId = "movies_mvc_client",
                       ClientName = "Movies MVC Web App",
                       ClientSecrets =   { new Secret("secret".Sha256()) },
                       AllowedGrantTypes = GrantTypes.Code,
                       //RequirePkce = false,
                       AllowOfflineAccess = true,

                       RequireConsent = true,
                       RedirectUris =  {"https://localhost:5002/signin-oidc"  },
                       PostLogoutRedirectUris ={ "https://localhost:5002/signout-callback-oidc" },

                       AllowedScopes =
                       {
                           //IdentityServerConstants.StandardScopes.OpenId,
                           //IdentityServerConstants.StandardScopes.Profile,
                           //IdentityServerConstants.StandardScopes.Address,
                           //IdentityServerConstants.StandardScopes.Email,
                           "openid",
                           "profile",
                           "address",
                           "email",
                           "movieAPI.read",
                           "roles",
                           "subscriptionlevel"
                       }
                   },
                   new Client
                   {
                        ClientName="Movies Api",
                        ClientId = "movieApiClient",
                        AllowedGrantTypes = GrantTypes.ClientCredentials,
                        //RequirePkce = false,
                        //AllowRememberConsent = false,
                        ClientSecrets =
                        {
                            new Secret("secret".Sha256())
                        },
                        AllowedScopes = {
                           //IdentityServerConstants.StandardScopes.OpenId,
                           //IdentityServerConstants.StandardScopes.Profile,
                           //IdentityServerConstants.StandardScopes.Address,
                           "openid",
                           "profile",
                           "address",
                           "email",
                           "movieAPI.read",
                           "roles",
                           "subscriptionlevel"



                       }
                   }

            };

        public static IEnumerable<ApiScope> ApiScopes =>
                   new ApiScope[]
                   {
                     new ApiScope("movieAPI.read"),
                     new ApiScope("movieAPI.write"),

                   };

        public static IEnumerable<ApiResource> ApiResources =>
          new[]
          {
              new ApiResource("movieAPI")
              {
                    Scopes = new List<string> { "movieAPI.read", "movieAPI.write"},
                    ApiSecrets = new List<Secret> {new Secret("ScopeSecret".Sha256())},
                    UserClaims = new List<string> {"role", "subscriptionlevel" }
              }




          };

        public static IEnumerable<IdentityResource> IdentityResources =>
          new IdentityResource[]
          {
              new IdentityResources.OpenId(),
              new IdentityResources.Profile(),
              new IdentityResources.Address(),
              new IdentityResources.Email(),
              new IdentityResource(
                    "roles",
                    "Your role(s)",
                    new List<string>() { "role" }),

              new IdentityResource(
                    "subscriptionlevel",
                    "Your subscription level",
                    new List<string> { "subscriptionlevel" })

          };

        public static List<TestUser> TestUsers =>
            new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "5BE86359-073C-434B-AD2D-A3932222DABE",
                    Username = "KNS70",
                    Password = "123456",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "iman"),
                        new Claim(JwtClaimTypes.FamilyName, "solouki"),
                        new Claim(JwtClaimTypes.Address, "kian shahr 1"),
                        new Claim(JwtClaimTypes.Role, "admin"),
                        new Claim("subscriptionlevel", "a1"),


                    }
                },
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "MGH",
                    Password = "654321",
                    Claims = new List<Claim>
                    {

                        new Claim(JwtClaimTypes.GivenName, "mohsen"),
                        new Claim(JwtClaimTypes.FamilyName, "ghalavand"),
                        new Claim(JwtClaimTypes.Address, "kian shahr 2"),
                        new Claim(JwtClaimTypes.Role, "b1_user"),
                        new Claim("subscriptionlevel", "b1"),

                    }
                },
                 new TestUser
                {
                    SubjectId = "f9539694-77e7-4dfe-84da-b4256e1ff7hy",
                    Username = "fafa",
                    Password = "123456",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "fafa"),
                        new Claim(JwtClaimTypes.FamilyName, "alipoor"),
                        new Claim(JwtClaimTypes.Address, "kian shahr 3"),
                        new Claim("subscriptionlevel", "c1"),

                    }
                }
            };
    }
}