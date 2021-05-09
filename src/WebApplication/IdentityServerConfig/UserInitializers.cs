using System;
using System.Threading.Tasks;
using AspNetCore.AsyncInitialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.IdentityServerConfig
{
    public class UserInitializer: IAsyncInitializer
    {
        private readonly IServiceProvider _serviceProvider;
        
        public UserInitializer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task InitializeAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ApplicationDbContext>();
            if (dbContext == null)
            {
                throw 
                    new InvalidOperationException("ApplicationDbContext must be registered in Service Collection but it wasn't");
            }

            await dbContext.Database.MigrateAsync();
            await Seed(scope);
        }

        private async Task Seed(IServiceScope scope)
        {
            await SeedData.EnsureUser(scope, "alice", "P@ssW0rd123!#");
        }
    }
}