using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic;
using WebApplication.Data;
using WebApplication.Models;

namespace WebApplication.IdentityServerConfig
{
    public class SeedData
    {
        public static async Task<ApplicationUser> EnsureUser(IServiceScope serviceScope, 
            string userName, string password)
        {
            var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

            if(userManager == null)
            {
                throw new InvalidOperationException("UserManager<ApplicationUser> must be registered and it wasn't");
            }
            
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    EmailConfirmed = true
                };
                
                var identityResult = await userManager.CreateAsync(user, password);
                if (!identityResult.Succeeded)
                {
                    throw new Exception("The password is probably not strong enough!");
                }
            }

            return user;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
            string uid, string role)
        {
            IdentityResult ir = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                ir = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            ir = await userManager.AddToRoleAsync(user, role);

            return ir;
        }
    }
}