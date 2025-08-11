using Microsoft.EntityFrameworkCore;

namespace UserMicroservice.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }
        public DbSet<Entities.User> Users { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
