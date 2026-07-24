using System.Reflection;
using System.Text;
using BasicArchitecture.UI.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace BasicArchitecture.UI.Extension;

public static class ProgramHelper
{
    // Repositories and services are auto-registered in DI via reflection — this file is
    // NOT touched when a new entity is added.
    public static IServiceCollection AddContainer(this IServiceCollection services)
    {
        RegisterBySuffix(services, typeof(CrudRepository<,>).Assembly, "Repository");
        RegisterBySuffix(services, typeof(CrudService<,>).Assembly, "Service");

        services.AddScoped(typeof(ICrudRepository<,>), typeof(CrudRepository<,>));
        services.AddScoped(typeof(IRangeRepository<,>), typeof(RangeRepository<,>));
        services.AddScoped(typeof(IBaseRepository<,>), typeof(BaseRepository<,>));
        services.AddScoped(typeof(ICrudService<,>), typeof(CrudService<,>));
        services.AddScoped(typeof(IBaseService<,>), typeof(BaseService<,>));

        return services;
    }

    private static void RegisterBySuffix(IServiceCollection services, Assembly assembly, string suffix)
    {
        var candidates = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition && t.Name.EndsWith(suffix, StringComparison.Ordinal));

        foreach (var type in candidates)
        {
            var iface = type.GetInterfaces().FirstOrDefault(i => i.Name == $"I{type.Name}");
            if (iface is not null)
                services.AddTransient(iface, type);
        }
    }

    public static IServiceCollection AddProfile(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddProfile<BasicArchitecture.Domain.Profile.ProjectProfile>());
        return services;
    }

    public static IServiceCollection AddContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BasicArchitecturedbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var domains = configuration.GetSection("JwtConfig:Domains").Get<string[]>() ?? Array.Empty<string>();
        services.AddCors(options =>
        {
            options.AddPolicy("DefaultCorsPolicy", policy =>
                policy.WithOrigins(domains).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
        });
        return services;
    }

    public static IServiceCollection AddHstsPolicy(this IServiceCollection services)
    {
        services.AddHsts(options =>
        {
            options.Preload = true;
            options.IncludeSubDomains = true;
            options.MaxAge = TimeSpan.FromDays(365);
        });
        return services;
    }

    public static IServiceCollection AddJWT(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["JwtConfig:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["JwtConfig:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Key"]!)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                        context.Response.Headers.Append("Token-Expired", "true");
                    return Task.CompletedTask;
                }
            };
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "BasicArchitecture.UI", Version = "v1" });
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Bearer {token}"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
                    Array.Empty<string>()
                }
            });
            options.OperationFilter<GenericResultOperationFilter>();
            options.EnableAnnotations();
        });

        return services;
    }
}
