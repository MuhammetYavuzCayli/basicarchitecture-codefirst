using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using System.Text;

namespace BasicArchitecture.UI.Extension;

// Runs AFTER UseAuthentication() (JwtBearer) has validated the token and populated
// context.User, but BEFORE UseAuthorization() — copies the frequently-used claims
// (UserId/BranchId/UserRoles) into HttpContext.Items so callers don't need to search
// User.Claims everywhere (ApplyEnforcedScope reads these).
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (context.User.Identity?.IsAuthenticated == true && token != null)
        {
            try
            {
                AttachUserToContext(context, token);
            }
            catch (SecurityTokenExpiredException)
            {
                context.Response.Headers.Append("Token-Expired", "true");

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "refresh" });
                return;
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { message = "Invalid token!" });
                return;
            }
        }

        await _next(context);
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"] ?? "SWWERDs3s2d1wewweQQW!!76348ÜÜÜĞÜQQwEEEEwSDDsEZXCZXCĞÜĞÜKuaforBerbarSaaSSecretKey_2026_Crypto!");

        var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _configuration["JwtConfig:Issuer"],
            ValidAudience = _configuration["JwtConfig:Audience"],
            ValidateLifetime = true, // Süre kontrolü aktif!
            ClockSkew = TimeSpan.Zero // Sunucu saat esnemesini sıfırlıyoruz ki tam 15. dk'da patlasın
        }, out SecurityToken validatedToken);

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new SecurityTokenException("Token içerisinde kullanıcı kimliği (NameIdentifier) bulunamadı.");
        var branchId = principal.FindFirst("BranchId")?.Value;

        context.Items["UserId"] = int.Parse(userId);
        context.Items["BranchId"] = string.IsNullOrEmpty(branchId) ? (int?)null : int.Parse(branchId);
        context.Items["UserRoles"] = principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToList();

        context.User = principal;
    }
}
