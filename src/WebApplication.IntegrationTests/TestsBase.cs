using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WebApplication.IntegrationTests
{
    public abstract class TestsBase : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected readonly WebApplicationFactory<Startup> WebApplicationFactory;

        protected readonly HttpClient Client;
        
        protected TestsBase(WebApplicationFactory<Startup> webApplicationFactory)
        {
            WebApplicationFactory = webApplicationFactory;
            Client = webApplicationFactory.CreateClient();
        }
    }
}