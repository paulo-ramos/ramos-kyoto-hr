namespace ramos_kyoto_hr.WebApi.Middleware;

/// <summary>
/// Extensões para configurar o middleware de tratamento de exceções
/// </summary>
public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

