using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Nexora.API.BackgroundServices;
using Nexora.Infrastructure.Database;
using Nexora.API.Middlewares;
using Nexora.Application.Interfaces;
using Nexora.Infrastructure.Data.Queries;
using Nexora.Application.Behaviors;
using Nexora.API.Validators;
using Nexora.Application.Features.Finance.GetTransactionHistory;
using Nexora.Infrastructure.Data.Repositories;

namespace Nexora.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' not found");

            builder.Services.AddScoped<ITransactionHistoryReader, TransactionHistoryReader>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();

            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetTransactionHistoryQuery).Assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(connectionString));

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

            builder.Services.AddValidatorsFromAssemblyContaining<GetTransactionHistoryQueryValidator>();

            builder.Services.AddAutoMapper(_ => {}, typeof(Program).Assembly);


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
