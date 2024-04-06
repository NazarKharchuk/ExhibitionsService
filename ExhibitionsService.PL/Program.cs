
using ExhibitionsService.BLL.Infrastructure.SeedData;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.BLL.Mapping;
using ExhibitionsService.BLL.Services;
using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using ExhibitionsService.DAL.Repositories;
using ExhibitionsService.PL.Mapping.Painter;
using ExhibitionsService.PL.Mapping.Painting;
using ExhibitionsService.PL.Mapping.Tag;
using ExhibitionsService.PL.Mapping.UserProfile;



//using ExhibitionsService.PL.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionsService.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddAuthorization();

            // DAL
            builder.Services.AddDbContext<ExhibitionContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<ExhibitionContext>()
                .AddDefaultTokenProviders();

            // BLL
            builder.Services.AddScoped<IPainterService, PainterService>();
            builder.Services.AddScoped<ITagService, TagService>();
            builder.Services.AddScoped<IPaintingService, PaintingService>();
            builder.Services.AddScoped<IUserProfileService, UserProfileService>();

            builder.Services.AddAutoMapper(typeof(PainterProfile), typeof(TagProfile), typeof(PaintingProfile));

            // PL
            builder.Services.AddAutoMapper(typeof(PainterModelsProfiles), typeof(TagModelsProfiles), typeof(PaintingModelsProfiles),
                typeof(UserProfilesProfiles));

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            //app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                try
                {
                    IdentityDataInitializer.SeedData(services, configuration).Wait();
                }
                catch (Exception exception)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, "Помилка на етапі ініціалізації бази даних початковими значеннями.");
                }
            }

            app.MapControllers();

            app.Run();
        }
    }
}
