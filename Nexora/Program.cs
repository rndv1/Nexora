using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Nexora.BackgroundServices;
using Nexora.Database;
using Nexora.DTOs.Validators;
using Nexora.Middlewares;
using Nexora.Services;

namespace Nexora
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' not found");

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(connectionString));
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IFinanceService, FinanceService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter the API token. Swagger adds the Bearer prefix automatically.",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "Token",
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                });
            });

            builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            builder.Services.AddControllers();
            builder.Services.AddHostedService<SessionCleanupService>();

            var app = builder.Build();

            await MigrateDatabaseAsync(app);

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseRouting();

            app.UseMiddleware<AuthorizationMiddleware>();

            app.MapControllers();

            await app.RunAsync();
        }

        private static async Task MigrateDatabaseAsync(WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();

            var dbContext = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            await dbContext.Database.MigrateAsync();
        }
    }
}
