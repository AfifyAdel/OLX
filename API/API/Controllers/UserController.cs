using Application.Models;
using Application.RequestsHandler.User;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator mediator;
        public UserController(IMediator mediator)
        {
            this.mediator = mediator;
        }
        [HttpPost("register")]
        public async Task<ActionResult<AuthUserDTO>> Register(Register.Command command)
        {
            return await mediator.Send(command);
        }
        [HttpPost("login")]
        public async Task<ActionResult<AuthUserDTO>> Login(Register.Command command)
        {
            return await mediator.Send(command);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("refresh")]
        public async Task<ActionResult<AuthUserDTO>> Refresh()
        {
            return await mediator.Send(new TokenRefresh.Query());
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("logout")]
        public async Task<ActionResult<bool>> Logout()
        {
            return await mediator.Send(new Logout.Query());
        }
    }
}
