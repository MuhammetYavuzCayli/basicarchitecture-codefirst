namespace BasicArchitecture.UI.Extension;

// Logs CRUD operations and errors from a single HTTP middleware, without hitting the DB —
// catches errors from both the generic CrudBaseController and custom endpoints (e.g.
// AccountController) in one place, without touching any service file.
public class CrudLoggingMiddleware
{
    private static readonly HashSet<string> MutatingMethods = new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "DELETE", "PATCH" };

    private readonly RequestDelegate _next;
    private readonly ILogger<CrudLoggingMiddleware> _logger;

    public CrudLoggingMiddleware(RequestDelegate next, ILogger<CrudLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var isMutating = MutatingMethods.Contains(context.Request.Method);
        try
        {
            await _next(context);
            if (isMutating)
            {
                _logger.LogInformation("CRUD {Method} {Path} -> {StatusCode} (UserId={UserId})",
                    context.Request.Method, context.Request.Path, context.Response.StatusCode, context.Items["UserId"] as int?);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CRUD {Method} {Path} failed (UserId={UserId})",
                context.Request.Method, context.Request.Path, context.Items["UserId"] as int?);
            throw;
        }
    }
}
