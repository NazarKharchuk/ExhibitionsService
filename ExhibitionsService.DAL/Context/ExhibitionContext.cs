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
        public DbSet<Painting> Paintings { get; set; }
        public DbSet<PaintingRating> PaintingRatings { get; set; }
        public DbSet<Exhibition> Exhibitions { get; set; }
        public DbSet<ExhibitionApplication> ExhibitionApplications { get; set; }
        public DbSet<Contest> Contests { get; set; }
        public DbSet<ContestApplication> ContestApplications { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Style> Styles { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PaintingLike> PaintingLikes { get; set; }

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
                
                builder.HasMany(up => up.VotedContestApplications)
                    .WithMany(ca => ca.Voters)
                    .UsingEntity<Dictionary<string, object>>(
                        "ContestsApplicationsVotes",
                        j => j.HasOne<ContestApplication>().WithMany().HasForeignKey("ApplicationId")
                            .OnDelete(DeleteBehavior.Cascade),
                        j => j.HasOne<UserProfile>().WithMany().HasForeignKey("ProfileId")
                            .OnDelete(DeleteBehavior.NoAction)
                    );
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

                builder.HasMany(t => t.Paintings)
                    .WithMany(p => p.Tags)
                    .UsingEntity("TagsPaintings");

                builder.HasMany(t => t.Exhibitions)
                    .WithMany(e => e.Tags)
                    .UsingEntity("TagsExhibitions");

                builder.HasMany(t => t.Contests)
                    .WithMany(c => c.Tags)
                    .UsingEntity("TagsContests");
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
                builder.Property(p => p.IsSold);
                builder.Property(p => p.Price);

                builder.HasOne(p => p.Painter)
                    .WithMany(pr => pr.Paintings)
                    .HasForeignKey(p => p.PainterId);

                builder.HasMany(p => p.Likers)
                    .WithMany(up => up.LikedPaintings)
                    .UsingEntity<PaintingLike>(
                        j => j
                            .HasOne(pl => pl.Profile)
                            .WithMany(p => p.PaintingLikes)
                            .HasForeignKey(pl => pl.ProfileId)
                            .OnDelete(DeleteBehavior.NoAction),
                        j => j
                            .HasOne(pl => pl.Painting)
                            .WithMany(p => p.PaintingLikes)
                            .HasForeignKey(pl => pl.PaintingId)
                            .OnDelete(DeleteBehavior.Cascade),
                        j =>
                        {
                            j.ToTable("PaintingsLikes").HasKey(j => new {j.PaintingId, j.ProfileId});
                            j.Property(j => j.AddedTime).IsRequired();
                        }
                    );
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

            modelBuilder.Entity<Exhibition>(builder =>
            {
                builder.ToTable("Exhibitions").HasKey(e => e.ExhibitionId);
                builder.Property(e => e.ExhibitionId).ValueGeneratedOnAdd();
                builder.Property(e => e.Name).IsRequired().HasMaxLength(50);
                builder.Property(e => e.Description).IsRequired().HasMaxLength(500);
                builder.Property(e => e.AddedDate).IsRequired();
                builder.Property(e => e.NeedConfirmation).IsRequired();
                builder.Property(e => e.PainterLimit);

                builder.HasMany(e => e.Paintings)
                    .WithMany(p => p.Exhibitions)
                    .UsingEntity<ExhibitionApplication>(
                        j => j
                            .HasOne(ea => ea.Painting)
                            .WithMany(p => p.ExhibitionApplications)
                            .HasForeignKey(ea => ea.PaintingId),
                        j => j
                            .HasOne(ea => ea.Exhibition)
                            .WithMany(e => e.Applications)
                            .HasForeignKey(ea => ea.ExhibitionId),
                        j =>
                        {
                            j.ToTable("ExhibitionApplications").HasKey(j => j.ApplicationId);
                            j.Property(j => j.ApplicationId).ValueGeneratedOnAdd();
                            j.Property(j => j.IsConfirmed).IsRequired();
                        }
                    );
            });

            modelBuilder.Entity<Contest>(builder =>
            {
                builder.ToTable("Contests").HasKey(c => c.ContestId);
                builder.Property(c => c.ContestId).ValueGeneratedOnAdd();
                builder.Property(c => c.Name).IsRequired().HasMaxLength(50);
                builder.Property(c => c.Description).IsRequired().HasMaxLength(500);
                builder.Property(c => c.AddedDate).IsRequired();
                builder.Property(c => c.StartDate).IsRequired();
                builder.Property(c => c.EndDate).IsRequired();
                builder.Property(c => c.NeedConfirmation).IsRequired();
                builder.Property(c => c.PainterLimit);
                builder.Property(c => c.WinnersCount).IsRequired();
                builder.Property(c => c.VotesLimit);

                builder.HasMany(c => c.Paintings)
                    .WithMany(p => p.Contests)
                    .UsingEntity<ContestApplication>(
                        j => j
                            .HasOne(ca => ca.Painting)
                            .WithMany(p => p.ContestApplications)
                            .HasForeignKey(ca => ca.PaintingId),
                        j => j
                            .HasOne(ca => ca.Contest)
                            .WithMany(c => c.Applications)
                            .HasForeignKey(ca => ca.ContestId),
                        j =>
                        {
                            j.ToTable("ContestApplications").HasKey(j => j.ApplicationId);
                            j.Property(j => j.ApplicationId).ValueGeneratedOnAdd();
                            j.Property(j => j.IsConfirmed).IsRequired();
                        }
                    );
            });

            modelBuilder.Entity<Genre>(builder =>
            {
                builder.ToTable("Genres").HasKey(x => x.GenreId);
                builder.Property(x => x.GenreId).ValueGeneratedOnAdd();
                builder.Property(x => x.GenreName).IsRequired().HasMaxLength(50);
                builder.HasIndex(x => x.GenreName).IsUnique();

                builder.HasMany(x => x.Paintings)
                    .WithMany(p => p.Genres)
                    .UsingEntity("GenresPaintings");
            });

            modelBuilder.Entity<Style>(builder =>
            {
                builder.ToTable("Styles").HasKey(x => x.StyleId);
                builder.Property(x => x.StyleId).ValueGeneratedOnAdd();
                builder.Property(x => x.StyleName).IsRequired().HasMaxLength(50);
                builder.HasIndex(x => x.StyleName).IsUnique();

                builder.HasMany(x => x.Paintings)
                    .WithMany(p => p.Styles)
                    .UsingEntity("StylesPaintings");
            });

            modelBuilder.Entity<Material>(builder =>
            {
                builder.ToTable("Materials").HasKey(x => x.MaterialId);
                builder.Property(x => x.MaterialId).ValueGeneratedOnAdd();
                builder.Property(x => x.MaterialName).IsRequired().HasMaxLength(50);
                builder.HasIndex(x => x.MaterialName).IsUnique();

                builder.HasMany(x => x.Paintings)
                    .WithMany(p => p.Materials)
                    .UsingEntity("MaterialsPaintings");
            });
        }
    }
}
