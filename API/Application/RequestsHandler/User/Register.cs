using Application.Interfaces;
using Application.Models;
using Domain;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Application.RequestsHandler.User
{
    public class Register
    {
        public class Command : IRequest<AuthUserDTO>
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }
        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.FirstName).NotEmpty().WithMessage("Your name is required").MinimumLength(2).WithMessage("Min length is 2").MaximumLength(10).WithMessage("Max length is 10");
                RuleFor(x => x.LastName).NotEmpty().WithMessage("Your name is required").MinimumLength(2).WithMessage("Min length is 2").MaximumLength(10).WithMessage("Max length is 10");
                RuleFor(x => x.UserName).NotEmpty().WithMessage("Your UserName is required").MinimumLength(5).WithMessage("Min length is 5").MaximumLength(18).WithMessage("Max length is 18").Matches("^[a-z]([a-zA-z0-9_]){5,18}$").WithMessage("Must start with lower case and letter, numbers and underscore are allowed with minimum length 5 and maxlenght of 18");

                RuleFor(x => x.Password).NotEmpty().WithMessage("Your password is required").MinimumLength(6).WithMessage("Min length is 6");
            }
        }
        public class Handler : IRequestHandler<Command, AuthUserDTO>
        {
            private readonly DataContext dataContext;
            private readonly UserManager<AppUser> userManager;
            private readonly RoleManager<Role> roleManager;
            private readonly IJwtGenerator jwtGenerator;
            private readonly IRefreshTokenGenerator refreshTokenGenerator;
            private readonly IHttpContextAccessor contextAccessor;
            private readonly IAuthCookies authCookies;

            public Handler(DataContext dataContext, UserManager<AppUser> userManager,RoleManager<Role> roleManager,
                IJwtGenerator jwtGenerator, IRefreshTokenGenerator refreshTokenGenerator,
                IHttpContextAccessor contextAccessor,IAuthCookies authCookies)
            {
                this.dataContext = dataContext;
                this.userManager = userManager;
                this.roleManager = roleManager;
                this.jwtGenerator = jwtGenerator;
                this.refreshTokenGenerator = refreshTokenGenerator;
                this.contextAccessor = contextAccessor;
                this.authCookies = authCookies;
            }
            public async Task<AuthUserDTO> Handle(Command request, CancellationToken cancellationToken)
            {
                var isExist = await dataContext.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName) != null;

                if (isExist)
                    throw new Exception("UserName already exist");

                var user = new AppUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName
                };

                var response = await userManager.CreateAsync(user,request.Password);
                var roleResult = await userManager.AddToRoleAsync(user, "Normal");
                if (response.Succeeded && roleResult.Succeeded)
                {
                    var refreshToken = refreshTokenGenerator.Generate(user.UserName);
                    await dataContext.RefreshTokens.AddAsync(new RefreshToken
                    {
                        Token = refreshToken,
                        CreatedAt = DateTime.UtcNow,
                        AppUser = user,
                        ExpireAt = DateTime.UtcNow.AddDays(2)
                    });
                    var success = await dataContext.SaveChangesAsync() > 0;
                    if (success)
                    {
                        await authCookies.SendAuthCookies(user, refreshToken);
                        return new AuthUserDTO(user);
                    }
                }
                throw new Exception("Server error - Register");
            }
        }
    }
}
