using Application.Models;
using Application.RequestsHandler.User;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
