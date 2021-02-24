using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Middlewares;
using API.Services;
using Application.Interfaces;
using Application.RequestsHandler.User;
using Domain;
using FluentValidation.AspNetCore;
using Infrastructure.Tokens;
using Infrastructure.User;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Persistence;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddUserIdentity();
            services.AddPostgreSQL(Configuration);
            services.AddMediatR(typeof(Register.Handler).Assembly);
            services.AddJwt(Configuration);
            services.AddControllers().AddNewtonsoftJson();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            //});
            services.AddMvc().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Register>());

            services.AddHttpContextAccessor();
            services.AddScoped<IJwtGenerator,JwtGenerator>();
            services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
            services.AddScoped<ICurrentUser, CurrentUser>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IAuthCookies, AuthCookies>();
            services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<ErrorHandlingMiddleware>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
