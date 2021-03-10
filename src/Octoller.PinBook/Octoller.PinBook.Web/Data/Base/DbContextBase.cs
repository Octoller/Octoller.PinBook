using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Octoller.PinBook.Web.Data.Base
{
    public class DbContextBase<TUser, TContext> : IdentityDbContext<TUser> 
        where TUser : IdentityUser
        where TContext : DbContext
    {
        public DbContextBase(DbContextOptions<TContext> options) 
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(Startup).Assembly);
            base.OnModelCreating(builder);
        }
    }
}
