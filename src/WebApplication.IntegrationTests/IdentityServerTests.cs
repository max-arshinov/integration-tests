using System.Net;
using System.Threading.Tasks;
using IdentityModel.Client;
using Xunit;
using Xunit.Abstractions;

namespace WebApplication.IntegrationTests
{
    public class IdentityServerTests: TestsBase
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public IdentityServerTests(MyApplicationFactory myApplicationFactory, ITestOutputHelper testOutputHelper) : 
            base(myApplicationFactory)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task GetDiscoveryDocumentAsync_Returns200()
        {
            var client = await GetClient();   
            //https://localhost:5001/.well-known/openid-configuration
            var document = await client.GetDiscoveryDocumentAsync();
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
            var client = await GetClient();
            var tokenResponse = await GetTokenResponseAsync();
            client.SetBearerToken(tokenResponse.AccessToken);
            var response = await client.GetAsync("/WeatherForecast");
            _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        
        [Fact]
        public async Task WeatherController_GetWithPassword_Returns200()
        {
            var client = await GetClient();
            var tokenResponse = await GetTokenResponseWithPasswordAsync();
            client.SetBearerToken(tokenResponse.AccessToken);
            var response = await client.GetAsync("/WeatherForecast");
            _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task WeatherController_GetApi3_Returns200()
        {
            var client = await GetClient();
            var tokenResponse = await GetTokenResponseAsync();
            client.SetBearerToken(tokenResponse.AccessToken);
            var response = await client.GetAsync("/WeatherForecast/Api3");
            _testOutputHelper.WriteLine(await response.Content.ReadAsStringAsync());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        private async Task<TokenResponse> GetTokenResponseWithPasswordAsync()
        {
            var client = await GetClient();
            var disco = await client.GetDiscoveryDocumentAsync();
            // https://docs.identityserver.io/en/release/quickstarts/2_resource_owner_passwords.html
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = "WebApplication",
                // ClientSecret = "secret",
                UserName = "alice",
                Password = "P@ssW0rd123!#",
                Scope = "api1"
            });

            return tokenResponse;
        }
        

        private async Task<TokenResponse> GetTokenResponseAsync()
        {
            var client = await GetClient();
            var disco = await client.GetDiscoveryDocumentAsync();
            
            // https://docs.identityserver.io/en/release/quickstarts/2_resource_owner_passwords.html
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
            {
                Address = disco.TokenEndpoint,
                ClientId = "WebApplication", //"ro.client",
                ClientSecret = "secret",
                UserName = "alice",
                Password = "password",
                Scope = "api1"
            });
            
            // var tokenResponse = await Client.RequestClientCredentialsTokenAsync(
            //     new ClientCredentialsTokenRequest
            //     {
            //         Address = disco.TokenEndpoint,
            //         ClientId = "client",
            //         ClientSecret = "secret",
            //         Scope = "api1"
            //     });
            return tokenResponse;
        }

        [Fact]
        public async Task Authorize_Returns401()
        {
            var client = await GetClient();
            var response = await client.GetAsync("/WeatherForecast");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);    
        }
    }
}