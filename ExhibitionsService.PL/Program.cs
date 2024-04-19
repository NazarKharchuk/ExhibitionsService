
using ExhibitionsService.BLL.Infrastructure.SeedData;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.BLL.Mapping;
using ExhibitionsService.BLL.Services;
using ExhibitionsService.DAL.Context;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using ExhibitionsService.DAL.Repositories;
using ExhibitionsService.PL.Mapping.Contest;
using ExhibitionsService.PL.Mapping.ContestApplication;
using ExhibitionsService.PL.Mapping.Exhibition;
using ExhibitionsService.PL.Mapping.ExhibitionApplication;
using ExhibitionsService.PL.Mapping.Genre;
using ExhibitionsService.PL.Mapping.Helper;
using ExhibitionsService.PL.Mapping.Material;
using ExhibitionsService.PL.Mapping.Painter;
using ExhibitionsService.PL.Mapping.Painting;
using ExhibitionsService.PL.Mapping.PaintingRating;
using ExhibitionsService.PL.Mapping.Style;
using ExhibitionsService.PL.Mapping.Tag;
using ExhibitionsService.PL.Mapping.UserProfile;
using ExhibitionsService.PL.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace ExhibitionsService.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CORSPolicy",
                    builder =>
                    {
                        builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()
                        .WithExposedHeaders("X-Pagination-Total-Count")
                        .SetIsOriginAllowed(origin => true);
                    });
            });

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
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IPaintingRatingService, PaintingRatingService>();
            builder.Services.AddScoped<IExhibitionService, ExhibitionService>();
            builder.Services.AddScoped<IExhibitionApplicationService, ExhibitionApplicationService>();
            builder.Services.AddScoped<IContestService, ContestService>();
            builder.Services.AddScoped<IContestApplicationService, ContestApplicationService>();
            builder.Services.AddScoped<IGenreService, GenreService>();
            builder.Services.AddScoped<IStyleService, StyleService>();
            builder.Services.AddScoped<IMaterialService, MaterialService>();

            builder.Services.AddAutoMapper(typeof(PainterProfile), typeof(TagProfile), typeof(PaintingProfile),
                typeof(PaintingRatingProfile), typeof(ExhibitionProfile), typeof(ExhibitionApplicationProfile),
                typeof(ContestProfile), typeof(ContestApplicationProfile), typeof(PaintingLikeProfile),
                typeof(GenreProfile), typeof(StyleProfile), typeof(MaterialProfile));

            // PL
            builder.Services.AddAutoMapper(typeof(PainterModelsProfiles), typeof(TagModelsProfiles), typeof(PaintingModelsProfiles),
                typeof(UserProfilesProfiles), typeof(AuthorizationProfiles), typeof(PaintingRatingModelsProfiles),
                typeof(ExhibitionModelsProfiles), typeof(ExhibitionApplicationModelsProfiles), typeof(ContestModelsProfiles),
                typeof(ContestApplicationModelsProfiles), typeof(GenreModelsProfiles), typeof(StyleModelsProfiles),
                typeof(MaterialModelsProfiles));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Exhibition service",
                    Description = "Asp.Net core Web API"
                });
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Будь ласка, введіть валідний токен доступу.",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            app.UseMiddleware<GlobalErrorHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("CORSPolicy");

            app.UseAuthentication();
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
