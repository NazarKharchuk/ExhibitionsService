using ExhibitionsService.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.DAL.Context
{
    public class ExhibitionContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Painter> Painters { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Painting> Paintings { get; set; }
        public DbSet<PaintingRating> PaintingRatings { get; set; }

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
                builder.HasIndex(p => p.Pseudonym).IsUnique();

                builder.HasOne(p => p.UserProfile)
                    .WithOne(up => up.Painter)
                    .HasForeignKey<Painter>(p => p.ProfileId)
                    .IsRequired();
            });

            modelBuilder.Entity<Tag>(builder =>
            {
                builder.ToTable("Tags").HasKey(t => t.TagId);
                builder.Property(t => t.TagId).ValueGeneratedOnAdd();
                builder.Property(t => t.TagName).IsRequired().HasMaxLength(20);
                builder.HasIndex(t => t.TagName).IsUnique();

            });

            modelBuilder.Entity<Painting>(builder =>
            {
                builder.ToTable("Paintings").HasKey(p => p.PaintingId);
                builder.Property(p => p.PaintingId).ValueGeneratedOnAdd();
                builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
                builder.Property(p => p.Description).IsRequired().HasMaxLength(500);
                builder.Property(p => p.CretionDate).IsRequired();
                builder.Property(p => p.Width).IsRequired();
                builder.Property(p => p.Height).IsRequired();
                builder.Property(p => p.ImagePath).IsRequired().HasMaxLength(100);
                builder.Property(p => p.Location).HasMaxLength(100);

                builder.HasOne(p => p.Painter)
                    .WithMany(pr => pr.Paintings)
                    .HasForeignKey(p => p.PainterId);
            });

            modelBuilder.Entity<PaintingRating>(builder =>
            {
                builder.ToTable("PaintingRatings").HasKey(r => r.RatingId);
                builder.Property(r => r.RatingId).ValueGeneratedOnAdd();
                builder.Property(r => r.RatingValue).IsRequired();
                builder.Property(r => r.Comment).HasMaxLength(500);
                builder.Property(r => r.AddedDate).IsRequired();

                builder.HasOne(r => r.UserProfile)
                    .WithMany(u => u.Ratings)
                    .HasForeignKey(r => r.ProfileId)
                    .OnDelete(DeleteBehavior.NoAction);

                builder.HasOne(r => r.Painting)
                    .WithMany(u => u.Ratings)
                    .HasForeignKey(r => r.PaintingId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
