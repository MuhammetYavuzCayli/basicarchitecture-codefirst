using BasicArchitecture.Service.Interfaces;

namespace BasicArchitecture.Service.Services;

// Sits OUTSIDE the generic CRUD pattern — Register/Login/Refresh flows manage several tables
// (User, PasswordHistory, UserToken, Role) in a single operation, so this works directly
// against the injected DbContext instead of going through the repository layer.
public class AuthService : IAuthService
{
    private static readonly TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(7);

    private readonly BasicArchitecturedbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(BasicArchitecturedbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<Result<bool>> RegisterAsync(RegisterDto dto)
    {
        var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == dto.BranchId && b.IsActive);
        if (branch is null)
            return new Result<bool>(false, ResultTypeEnum.NotFound, false, "Branch not found.");

        var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
        if (emailExists)
            return new Result<bool>(false, ResultTypeEnum.Error, false, "This email address is already registered.");

        var roleId = dto.RoleId ?? Constants.Defaults.DefaultRoleId;
        var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId && r.IsActive);
        if (!roleExists)
            return new Result<bool>(false, ResultTypeEnum.NotFound, false, "Role not found.");

        var user = new User
        {
            BranchId = dto.BranchId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            IsActive = true
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _context.PasswordHistories.Add(new PasswordHistory
        {
            UserId = user.Id,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IsActive = true
        });
        await _context.SaveChangesAsync();

        await _context.Database.ExecuteSqlInterpolatedAsync(
            $"INSERT INTO \"UserRole\" (\"UserId\", \"RoleId\") VALUES ({user.Id}, {roleId})");

        return new Result<bool>(true, ResultTypeEnum.Success, true, "Registration successful.");
    }

    public async Task<Result<TokenResponseDto>> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.Branch)
            .FirstOrDefaultAsync(u => u.Email == dto.UserName);

        if (user is null || !user.IsActive)
            return new Result<TokenResponseDto>(false, ResultTypeEnum.Error, null!, "Invalid username or password.");

        var lastPassword = await _context.PasswordHistories
            .Where(p => p.UserId == user.Id && p.IsActive)
            .OrderByDescending(p => p.CreateDate)
            .FirstOrDefaultAsync();

        if (lastPassword is null || !BCrypt.Net.BCrypt.Verify(dto.Password, lastPassword.PasswordHash))
            return new Result<TokenResponseDto>(false, ResultTypeEnum.Error, null!, "Invalid username or password.");

        var roleNames = await GetRoleNamesAsync(user.Id);
        var (accessToken, expiration) = GenerateAccessToken(user, roleNames);
        var refreshToken = await IssueRefreshTokenAsync(user.Id);

        var response = new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = expiration,
            RoleName = roleNames.FirstOrDefault() ?? string.Empty,
            Account = MapAccount(user, roleNames)
        };

        return new Result<TokenResponseDto>(true, ResultTypeEnum.Success, response, string.Empty);
    }

    public async Task<Result<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto)
    {
        ClaimsPrincipal principal;
        try
        {
            principal = GetPrincipalFromExpiredToken(dto.AccessToken);
        }
        catch
        {
            return new Result<TokenResponseDto>(false, ResultTypeEnum.Error, null!, "Invalid access token.");
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
            return new Result<TokenResponseDto>(false, ResultTypeEnum.Error, null!, "Invalid access token.");

        var existingToken = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.RefreshToken == dto.RefreshToken);

        if (existingToken is null || existingToken.ExpireDate <= DateTime.UtcNow)
            return new Result<TokenResponseDto>(false, ResultTypeEnum.Error, null!, "Refresh token is invalid or has expired.");

        var user = await _context.Users.Include(u => u.Branch).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null || !user.IsActive)
            return new Result<TokenResponseDto>(false, ResultTypeEnum.Error, null!, "User not found.");

        _context.UserTokens.Remove(existingToken);
        await _context.SaveChangesAsync();

        var roleNames = await GetRoleNamesAsync(user.Id);
        var (accessToken, expiration) = GenerateAccessToken(user, roleNames);
        var refreshToken = await IssueRefreshTokenAsync(user.Id);

        var response = new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = expiration,
            RoleName = roleNames.FirstOrDefault() ?? string.Empty,
            Account = MapAccount(user, roleNames)
        };

        return new Result<TokenResponseDto>(true, ResultTypeEnum.Success, response, string.Empty);
    }

    public async Task<Result<AccountDto>> GetUserAsync(int userId)
    {
        var user = await _context.Users.Include(u => u.Branch).FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return new Result<AccountDto>(false, ResultTypeEnum.NotFound, null!, "User not found.");

        var roleNames = await GetRoleNamesAsync(user.Id);
        return new Result<AccountDto>(true, ResultTypeEnum.Success, MapAccount(user, roleNames), string.Empty);
    }

    private async Task<List<string>> GetRoleNamesAsync(int userId)
    {
        return await (from ur in _context.Set<Dictionary<string, object>>("UserRole")
                       join r in _context.Roles on (int)ur["RoleId"] equals r.Id
                       where (int)ur["UserId"] == userId
                       select r.Name).ToListAsync();
    }

    private async Task<string> IssueRefreshTokenAsync(int userId)
    {
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        _context.UserTokens.Add(new UserToken
        {
            UserId = userId,
            RefreshToken = refreshToken,
            ExpireDate = DateTime.UtcNow.Add(RefreshTokenLifetime)
        });
        await _context.SaveChangesAsync();
        return refreshToken;
    }

    private (string token, DateTime expiration) GenerateAccessToken(User user, List<string> roleNames)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new("BranchId", user.BranchId?.ToString() ?? string.Empty)
        };
        claims.AddRange(roleNames.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.Add(AccessTokenLifetime);

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtConfig:Issuer"],
            audience: _configuration["JwtConfig:Audience"],
            claims: claims,
            expires: expiration,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _configuration["JwtConfig:Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["JwtConfig:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Key"]!)),
            ValidateLifetime = false
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token.");
        }

        return principal;
    }

    private static AccountDto MapAccount(User user, List<string> roleNames) => new()
    {
        Id = user.Id,
        BranchId = user.BranchId,
        BranchName = user.Branch?.Name,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Roles = roleNames
    };
}
