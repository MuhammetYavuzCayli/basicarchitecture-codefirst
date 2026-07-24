namespace BasicArchitecture.Core.Models;

public class TokenResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime AccessTokenExpiration { get; set; }
    public string RoleName { get; set; } = null!;
    public AccountDto Account { get; set; } = null!;
}

public class RefreshTokenRequestDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
