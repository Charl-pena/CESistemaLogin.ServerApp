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
        
        // Permitir acceso a /authentication/login sin autenticación
        if (path.StartsWithSegments("/authentication/login", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/account/login", StringComparison.OrdinalIgnoreCase)
        )
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
            // context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            // Establece el código de estado HTTP a 302 para redirigir
            context.Response.StatusCode = StatusCodes.Status302Found;
            // Establece la URL de redirección
            context.Response.Headers.Location = "/authentication/login"; 
            // Finaliza el procesamiento de la solicitud
            return;
        }
    }
}
