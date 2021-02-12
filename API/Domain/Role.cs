using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Role : IdentityRole
    {
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
