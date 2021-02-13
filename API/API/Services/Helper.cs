using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public static class Helper
    {
        public static void AddUserIdentity(this IServiceCollection services)
        {
            var identity = services.AddIdentity<AppUser, Role>(opt =>
            {
                var passwordManager = opt.Password;
                passwordManager.RequireDigit = false;
                passwordManager.RequireLowercase = false;
                passwordManager.RequireNonAlphanumeric = false;
                passwordManager.RequireUppercase = false;
            });
            var identityBuilder = new IdentityBuilder(identity.UserType, typeof(Role), identity.Services);
            identityBuilder.AddEntityFrameworkStores<DataContext>();
            identityBuilder.AddUserManager<UserManager<AppUser>>();
            identityBuilder.AddSignInManager<SignInManager<AppUser>>();
            identityBuilder.AddRoleValidator<RoleValidator<Role>>();
            identityBuilder.AddRoleManager<RoleManager<Role>>();
        }

        public static void AddPostgreSQL(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseNpgsql(configuration.GetConnectionString("default"));
            });
        }
    }
}
