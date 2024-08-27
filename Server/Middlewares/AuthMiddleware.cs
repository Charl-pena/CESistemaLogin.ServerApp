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

        // Rutas que no requieren autenticación
        if (IsPublicRoute(path))
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
            // Redirigir a la página de inicio de sesión
            context.Response.StatusCode = StatusCodes.Status302Found;
            context.Response.Headers.Location = "/authentication/login";
            return;
        }
    }

    private static bool IsPublicRoute(PathString path)
    {
        return path.StartsWithSegments("/authentication/login", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWithSegments("/account/login", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWithSegments("/authentication/register", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWithSegments("/account/register", StringComparison.OrdinalIgnoreCase);
    }
}
