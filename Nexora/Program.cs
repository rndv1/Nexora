using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Nexora.Database;
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
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IFinanceService, FinanceService>();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Please insert api token",
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

            builder.Services.AddControllers();
            builder.Services.AddHostedService<SessionCleanupService>();

            var app = builder.Build();
            var scopeContainer = app.Services.CreateScope();
            using (scopeContainer)
            {
                var dbContainer = scopeContainer.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContainer.Database.MigrateAsync();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseMiddleware<AuthorizationMiddleware>();
            
            app.MapControllers();

            app.Run();
        }
    }
}
