using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReviewsDashboard.Entities;
using ReviewsDashboard.Privilage;

namespace ReviewsDashboard.Context
{
    public class DbContainer: IdentityDbContext<ExtendIdentityUser, IdentityRole, string>
    {
        public DbContainer(DbContextOptions<DbContainer> ops): base(ops)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

        }
        public DbSet<Emails> email { get; set; }

        public DbSet<Review> review { get; set; }

        public DbSet<Properties> properties { get; set; }

        public DbSet<Business> business { get; set; }
    }
}
