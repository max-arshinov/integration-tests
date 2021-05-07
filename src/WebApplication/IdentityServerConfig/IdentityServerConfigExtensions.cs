using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Extensions.Mykeels.Services;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

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
        
        public static IServiceCollection AddInMemoryIdentityServer(this IServiceCollection services, 
            IWebHostEnvironment environment)
        {
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryApiScopes(ApiScopes)
                .AddInMemoryClients(Clients);

            services
                .AddAuthentication()
                .AddJwtBearer();

            services.Configure<JwtBearerOptions>(
                JwtBearerDefaults.AuthenticationScheme,
                options =>
                {
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.ValidateIssuerSigningKey = false;
                    options.TokenValidationParameters.RequireSignedTokens = false;
                    options.TokenValidationParameters.SignatureValidator =
                        delegate(string token, TokenValidationParameters parameters)
                        {
                            var jwt = new JwtSecurityToken(token);
                            return jwt;
                        };
                    
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = OnAuthenticationFailed,
                        OnTokenValidated = OnTokenValidated
                    };
                });
            
            return services;
        }

        private static Task OnTokenValidated(TokenValidatedContext context)
        {
            var claims = new List<Claim>
            {
                new Claim("groups", "ad")
            };
            var appIdentity = new ClaimsIdentity(claims);

            context.Principal.AddIdentity(appIdentity);


            return Task.CompletedTask;
        }
        
        private static Task OnAuthenticationFailed(AuthenticationFailedContext arg)
        {
            return Task.CompletedTask;
        }
    }
}