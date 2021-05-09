using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.AsyncInitialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication.IntegrationTests
{
    public class MyApplicationFactory
    {
        private readonly WebApplicationFactory<Startup> _webApplicationFactory = new();
        private HttpClient _httpClient;
        
        public async Task<HttpClient> GetClient()
        {
            if (_httpClient != null)
            {
                return _httpClient;
            }
            
            var usersInitializer = _webApplicationFactory.Services.GetService<IAsyncInitializer>();
            if (usersInitializer != null)
            {
                await usersInitializer.InitializeAsync();
            }

            _httpClient = _webApplicationFactory.CreateClient();
            return _httpClient;
        }
    }
}