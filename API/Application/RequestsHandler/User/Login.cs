using Application.Interfaces;
using Application.Models;
using Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IIS;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.RequestsHandler.User
{
    public class Login
    {
        public class Command : IRequest<AuthUserDTO>
        {
            public string UserName { get; set; }
            public string Password { get; set; }
        }
       
        public class Handler : IRequestHandler<Command, AuthUserDTO>
        {
            private readonly UserManager<AppUser> userManager;
            private readonly SignInManager<AppUser> signInManager;
            private readonly IJwtGenerator jwtGenerator;
            private readonly IRefreshTokenGenerator refreshTokenGenerator;
            private readonly IHttpContextAccessor contextAccessor;
            private readonly DataContext dataContext;
            private readonly IAuthCookies authCookies;

            public Handler(DataContext dataContext, UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,
                IJwtGenerator jwtGenerator, IRefreshTokenGenerator refreshTokenGenerator,
                IHttpContextAccessor contextAccessor, IAuthCookies authCookies)
            {
                this.userManager = userManager;
                this.signInManager = signInManager;
                this.jwtGenerator = jwtGenerator;
                this.refreshTokenGenerator = refreshTokenGenerator;
                this.contextAccessor = contextAccessor;
                this.dataContext = dataContext;
                this.authCookies = authCookies;
            }
            public async Task<AuthUserDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await userManager.FindByNameAsync(request.UserName);
                if(user == null)
                {
                    throw new Exception("UserName is not exist");
                }
                var response = await signInManager.CheckPasswordSignInAsync(user, request.Password,false);
                if (response.Succeeded)
                {
                    var refreshToken = refreshTokenGenerator.Generate(user.UserName);
                    var token = new RefreshToken
                    {
                        Token = refreshToken,
                        CreatedAt = DateTime.UtcNow,
                        AppUser = user,
                        ExpireAt = DateTime.UtcNow.AddDays(2)
                    };
                    await dataContext.RefreshTokens.AddAsync(token,cancellationToken);
                    var success = await dataContext.SaveChangesAsync() > 0;
                    if (success)
                    {
                        await authCookies.SendAuthCookies(user, token.Token);
                    }
                    return new AuthUserDTO(user);
                }
                else
                {
                    throw new Exception("Wrong password" );
                }
                throw new Exception("Server error -Login");
            }
        }
    }
}
