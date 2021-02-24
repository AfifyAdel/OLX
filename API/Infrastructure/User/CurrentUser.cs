using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.User
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor contextAccessor;
        public CurrentUser(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }
        public string UserId {
            get 
            {
                var id = contextAccessor.HttpContext.User.Claims?.FirstOrDefault(x => x.Type == "_cuser")?.Value;
                return id;
            }
        }
    }
}
