using ExhibitionsService.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Context
{
    public class ExhibitionContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<UserProfile> UserProfiles { get; set; }

        public ExhibitionContext(DbContextOptions<ExhibitionContext> options): base(options)
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(builder =>
            {
                builder.HasOne(u => u.UserProfile)
                    .WithOne(up => up.User)
                    .HasForeignKey<UserProfile>(up => up.UserId);
            });

            modelBuilder.Entity<UserProfile>(builder =>
            {
                builder.ToTable("UserProfiles").HasKey(x => x.ProfileId);
                builder.Property(x => x.ProfileId).ValueGeneratedOnAdd();
            });
        }
    }
}
