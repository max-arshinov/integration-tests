using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace WebApplication.IntegrationTests
{
    public abstract class TestsBase : IClassFixture<MyApplicationFactory>
    {
        private readonly MyApplicationFactory _myApplicationFactory;

        protected TestsBase(MyApplicationFactory myApplicationFactory)
        {
            _myApplicationFactory = myApplicationFactory;
        }

        public Task<HttpClient> GetClient() => _myApplicationFactory.GetClient();
    }
}