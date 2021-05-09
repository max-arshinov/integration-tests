using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Data;
using WebApplication.Extensions.Mykeels.Services;
using WebApplication.Models;
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
                },
                new Client
                {
                    ClientId = "ro.client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes = { "api1" }
                }
            };

        private static readonly List<TestUser> Users = new()
        {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password"
                }
            };

        public static IServiceCollection AddIdentityServerWithApiAuthorization(
            this IServiceCollection services)
        {
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            return services;
        }

        public static IServiceCollection AddInMemoryIdentityServer(
            this IServiceCollection services, 
            IWebHostEnvironment environment)
        {
            services
                .AddIdentityServer()
                .AddDeveloperSigningCredential()
                
                .AddInMemoryApiScopes(ApiScopes)
                .AddInMemoryClients(Clients)
                .AddTestUsers(Users);

            return services;
        }

        public static void AddJwt(this IServiceCollection services)
        {
            services
                .AddAuthentication()
                .AddIdentityServerJwt();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Api3", p => p.RequireClaim("Api3"));
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireClaim("TokenClain")
                    .Build();
            });

            services.Configure<JwtBearerOptions>(
                IdentityServerJwtConstants.IdentityServerJwtBearerScheme,
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
        }

        private static AuthorizationPolicyBuilder RequireClaim(AuthorizationPolicyBuilder policy)
        {
            return policy.RequireClaim("Api3");
        }

        private static Task OnTokenValidated(TokenValidatedContext context)
        {
            var claims = new List<Claim>
            {
                new Claim("TokenClain", "ad"),
                new Claim("Api3", "Api3"),
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