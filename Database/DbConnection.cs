using Microsoft.EntityFrameworkCore;
using Distributed_Cache.Models;

namespace Distributed_Cache.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
    }
}