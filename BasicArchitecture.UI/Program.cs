using BasicArchitecture.UI.Extension;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var logFilePath = ResolveLogPath(builder.Configuration["AppLogging:FilePath"] ?? "/var/www/logs/log-.txt");
var retainedFileCountLimit = builder.Configuration.GetValue<int?>("AppLogging:RetainedFileCountLimit") ?? 30;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .WriteTo.File(path: logFilePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: retainedFileCountLimit,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHstsPolicy();
builder.Services.AddContainer();
builder.Services.AddJWT(builder.Configuration);
builder.Services.AddProfile();
builder.Services.AddContext(builder.Configuration);
builder.Services.AddCorsPolicy(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddResponseCompression();

var app = builder.Build();

app.UseCors("DefaultCorsPolicy");
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<CrudLoggingMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

try
{
    Log.Information("Starting BasicArchitecture.UI.");
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

// The AppLogging:FilePath default ("/var/www/logs/log-.txt") works as-is on a Linux server;
// on Windows it is resolved against the current drive's root (C:\var\www\logs\...) — a
// deliberate choice so the same config value works in both environments.
static string ResolveLogPath(string configuredPath)
{
    if (!OperatingSystem.IsWindows() || !configuredPath.StartsWith('/'))
        return configuredPath;

    var driveRoot = Path.GetPathRoot(AppContext.BaseDirectory) ?? "C:\\";
    return Path.Combine(driveRoot, configuredPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
}
