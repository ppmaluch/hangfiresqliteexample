using HangfireSqliteExample.Model;
using Microsoft.EntityFrameworkCore;

namespace HangfireSqliteExample.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Person> Persons => Set<Person>();
    }
}
