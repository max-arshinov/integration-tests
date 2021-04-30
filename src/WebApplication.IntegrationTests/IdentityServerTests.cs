using System;
using System.Net;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace WebApplication.IntegrationTests
{
    public class IdentityServerTests: TestsBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public IdentityServerTests(WebApplicationFactory<Startup> webApplicationFactory, ITestOutputHelper testOutputHelper) : base(webApplicationFactory)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task GetDiscoveryDocumentAsync_Returns200()
        {
            //https://localhost:5001/.well-known/openid-configuration
            var document = await Client.GetDiscoveryDocumentAsync();
            Assert.False(document.IsError);
        }

        [Fact]
        public async Task RequestClientCredentialsTokenAsync_ReturnsToken()
        {
            var tokenResponse = await GetTokenResponseAsync();
            Assert.False(tokenResponse.IsError);
        }
        
        [Fact]
        public async Task WeatherController_Get_Returns200()
        {
            var tokenResponse = await GetTokenResponseAsync();
            Client.SetBearerToken(tokenResponse.AccessToken);
            
            var response = await Client.GetAsync("/WeatherForecast");
            _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<TokenResponse> GetTokenResponseAsync()
        {
            var disco = await Client.GetDiscoveryDocumentAsync();

            var tokenResponse = await Client.RequestClientCredentialsTokenAsync(
                new ClientCredentialsTokenRequest
                {
                    Address = disco.TokenEndpoint.Replace("http://localhost", ""),

                    ClientId = "client",
                    ClientSecret = "secret",
                    Scope = "api1"
                });
            return tokenResponse;
        }

        [Fact]
        public async Task Authorize_Returns401()
        {
            var response = await Client.GetAsync("/WeatherForecast");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);    
        }
    }
}