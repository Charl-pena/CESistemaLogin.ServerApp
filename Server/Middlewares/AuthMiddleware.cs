namespace CESistemaLogin.ServerApp.Server.Middlewares;

public class AuthMiddleware
{
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path;
        
        // Permitir acceso a /login sin autenticación
        if (path.StartsWithSegments("/login", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Verificar autenticación para otras rutas
        if (context.User.Identity?.IsAuthenticated ?? false)
        {
            await _next(context);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}
