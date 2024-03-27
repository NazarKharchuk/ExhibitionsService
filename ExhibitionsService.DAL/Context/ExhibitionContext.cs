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
            Database.EnsureCreated();
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
                builder.ToTable("UserProfiles").HasKey(up => up.ProfileId);
                builder.Property(up => up.ProfileId).ValueGeneratedOnAdd();
                builder.Property(up => up.FirstName).IsRequired().HasMaxLength(20);
                builder.Property(up => up.LastName).HasMaxLength(20);
                builder.Property(up => up.JoiningDate).IsRequired();

                /*builder.HasOne(up => up.Painter)
                    .WithOne(p => p.UserProfile)
                    .HasForeignKey<Painter>(p => p.ProfileId)
                    .IsRequired(false);*/
            });

            modelBuilder.Entity<Painter>(builder =>
            {
                builder.ToTable("Painters").HasKey(p => p.PainterId);
                builder.Property(p => p.PainterId).ValueGeneratedOnAdd();
                builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
                builder.Property(p => p.Pseudonym).IsRequired().HasMaxLength(20);

                builder.HasOne(p => p.UserProfile)
                    .WithOne(up => up.Painter)
                    .HasForeignKey<Painter>(p => p.ProfileId)
                    .IsRequired();
            });
        }
    }
}
