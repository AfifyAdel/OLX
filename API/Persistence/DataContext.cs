using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class DataContext : IdentityDbContext<AppUser,Role,string,IdentityUserClaim<string>,UserRoles,IdentityUserLogin<string>,
        IdentityRoleClaim<string>,IdentityUserToken<string>>
    {
        public DataContext(DbContextOptions dbContext) : base(dbContext)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRoles>(opt =>
            {
                opt.HasKey(x => new { x.RoleId, x.UserId });

                //Role
                opt.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).IsRequired();

                //User
                opt.HasOne(x => x.AppUser).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).IsRequired();
            });

            builder.Entity<RefreshToken>(opt =>
            {
                //User wirh Tokens
                opt.HasOne(x => x.AppUser).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.UserId);

                opt.HasIndex(x => x.Token);
            });
        }
    }
}
