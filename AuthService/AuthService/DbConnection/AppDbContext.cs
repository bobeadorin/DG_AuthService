using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.DbConnection
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
       .OwnsOne(u => u.ProfileData);
        }

        public DbSet<User> Users { get; set; }
    }
}
