using Microsoft.EntityFrameworkCore;
using Octoller.PinBook.Web.Data.Base;
using Octoller.PinBook.Web.Data.Model;

namespace Octoller.PinBook.Web.Data
{
    public class DatabaseAppContext : DbContextBase<User, DatabaseAppContext> 
    {
        public DatabaseAppContext(DbContextOptions<DatabaseAppContext> options) 
            : base(options)
        { }
    }
}
