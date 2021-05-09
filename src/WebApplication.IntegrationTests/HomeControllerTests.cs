using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using WebApplication.Controllers;
using Xunit;

namespace WebApplication.IntegrationTests
{
    public class HomeControllerTests: TestsBase
    {
        public HomeControllerTests(MyApplicationFactory myApplicationFactory) : base(myApplicationFactory) { }
        
        private async Task<HttpResponseMessage> CreateEntityAsync()
        {
            var client = await GetClient();  
            return await client.PostAsync(
                "Home/ResultCreated",
                JsonContent.Create(new Models.Data()
                {
                    A = "A",
                    B = "B"
                }));
        }

        // 201
        [Fact]
        public async Task Created_LocationHeaderPointsToCreatedEntity()
        {
            var client = await GetClient();  
            var response = await CreateEntityAsync();
            var uri = response.Headers.Location;
            Assert.NotNull(uri);

            var response2 = await client.GetAsync(uri);
            var data = await response2.Content.ReadFromJsonAsync<Models.Data>();
            Assert.NotNull(data);
            Assert.NotNull(data.A);
            Assert.NotNull(data.B);
        }
        
        // 415
        [Fact]
        public async Task Post_WrongContentType()
        {
            var client = await GetClient();  
            var res = await client.PostAsync(
                "Home/ResultCreated",
                new StringContent(""));
            
            Assert.Equal(HttpStatusCode.UnsupportedMediaType, res.StatusCode);
        }

        // 400. 422 might be a good idea but asp.net core uses 400 by default
        [Fact]
        public async Task PostCreatedInvalid_ReturnBadRequest()
        {
            var client = await GetClient();  
            var response = await client.PostAsync(
                "Home/ResultCreated",
                JsonContent.Create(new Models.Data()));
            
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        // 201
        [Fact]
        public async Task PostCreatedValid_ReturnsCreated()
        {
            var response = await CreateEntityAsync();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        // 308
        [Fact]
        public async Task PreserveMethodFrom_RedirectsWithBody()
        {
            var client = await GetClient();  
            var response = await client.PostAsJsonAsync(
                $"Home/{nameof(HomeController.PreserveMethodFrom)}", 
                new Models.Data()
            {
                A = "A",
                B = "B"
            });

            var data = await response.Content.ReadFromJsonAsync<Models.Data>();
            Assert.NotNull(data);
            Assert.Equal("A", data.A);
            Assert.Equal("B", data.B);
        }

        [Theory]
        [InlineData(nameof(HomeController.ResultOk), HttpStatusCode.OK)] // 200
        [InlineData(nameof(HomeController.ResultOkNotEmpty), HttpStatusCode.OK)] // 200
        [InlineData(nameof(HomeController.ResultNoContent), HttpStatusCode.NoContent)] // 204 
        [InlineData(nameof(HomeController.ResultOkNullNoContent), HttpStatusCode.NoContent)] // 204
        [InlineData(nameof(HomeController.ResultCreatedViaStatusCode), HttpStatusCode.Created)] // 201
        [InlineData("100500", HttpStatusCode.NoContent)] // 201
        [InlineData(nameof(HomeController.Exception), HttpStatusCode.InternalServerError)] // 500
        [InlineData(nameof(HomeController.RedirectFrom), HttpStatusCode.OK)] // 302 -> 200
        [InlineData(nameof(HomeController.MovedFrom), HttpStatusCode.OK)] // 301 -> 200
        [InlineData(nameof(HomeController.BadGateway), HttpStatusCode.BadGateway)] // 502
        [InlineData(nameof(HomeController.GatewayTimeout), HttpStatusCode.GatewayTimeout)] // 504
        [InlineData(nameof(HomeController.ReturnForbiddenWithoutAuthorizationHeader), HttpStatusCode.Forbidden)] // 403
        public async Task InvokeRoute_ReturnRightHttpCode(
            string url,
            HttpStatusCode httpStatusCode)
        {
            var client = await GetClient();  
            var response = await client.GetAsync($"Home/{url}");
            Assert.Equal(httpStatusCode, response.StatusCode);
        }
    }
}