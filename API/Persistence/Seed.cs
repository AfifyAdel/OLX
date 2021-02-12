using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class Seed
    {
        public async static Task AddData(UserManager<AppUser> userManager, RoleManager<Role> roleManager)
        {
            if (!await roleManager.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role
                    {
                        Name="Admin"
                    },
                    new Role
                    {
                        Name="Moderator"
                    },
                    new Role
                    {
                        Name="Seller"
                    },
                    new Role
                    {
                        Name="Normal"
                    }
                };
                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role);
                }
            }

            if (!await userManager.Users.AnyAsync())
            {
                var user = new AppUser()
                {
                    UserName = "Test",
                    Email = "test@test.com",
                    FirstName = "Test",
                    LastName = "Test"
                };

                await userManager.CreateAsync(user,"password");

                await userManager.AddToRoleAsync(user, "Normal");
            }
        }
    }
}
