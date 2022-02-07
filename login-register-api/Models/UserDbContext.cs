using Microsoft.EntityFrameworkCore;

namespace login_register_api.Models
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public UserDbContext(DbContextOptions options) :base(options) 
        {

        }
    }
}
