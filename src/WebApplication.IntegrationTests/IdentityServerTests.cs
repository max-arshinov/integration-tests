using System.Net;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WebApplication.IntegrationTests
{
    public class IdentityServerTests: TestsBase
    {
        public IdentityServerTests(WebApplicationFactory<Startup> webApplicationFactory) : base(webApplicationFactory)
        {
        }

        [Fact]
        public async Task GetDiscoveryDocumentAsync_Returns200()
        {
            //https://localhost:5001/.well-known/openid-configuration
            var document = await Client.GetDiscoveryDocumentAsync();
            Assert.False(document.IsError);
        }

        [Fact]
        public async Task Authorize_Returns401()
        {
            var response = await Client.GetAsync("/WeatherForecast");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);    
        }
    }
}