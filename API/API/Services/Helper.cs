using Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

        public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["secret_key"]));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = key,
                    SaveSigninToken = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                opt.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies.TryGetValue("_aid", out string jwt);
                        if (token)
                        {
                            context.Token = jwt;
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        if (context.Error != null)
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
