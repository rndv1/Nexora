using Microsoft.EntityFrameworkCore;
using Nexora.Attributes;
using Nexora.Database;

namespace Nexora.Middlewares;

public class AuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory;

    public AuthorizationMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory)
    {
        _next = next;
        _scopeFactory = scopeFactory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var attribute = endpoint?.Metadata.GetMetadata<MyAuthorizeAttribute>();
        if (attribute == null)
        {
            await _next(context);
            return;
        }

        var authorizationHeader = context.Request.Headers[Constants.Authorization].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(authorizationHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        
        var token = authorizationHeader.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
        if (session == null || session.ExpiresAt < DateTime.UtcNow)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
        
        context.Items[Constants.UserIdContextParameterName] = session.UserId;
        
        await _next(context);
    }
}