using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace WebApplication.IdentityServerConfig
{
    public static class IdentityServerConfigExtensions
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("api1", "My API")
            };
        
        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "api1" }
                }
            };
        
        public static IServiceCollection AddInMemoryIdentityServer(this IServiceCollection services)
        {
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()        
                //This is for dev only scenarios when you don’t have a certificate to use.
                .AddInMemoryApiScopes(ApiScopes)
                .AddInMemoryClients(Clients);
            
            services
                .AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    //options.Authority = "https://localhost:5001";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false
                    };
                });

            return services;
        }
    }
}