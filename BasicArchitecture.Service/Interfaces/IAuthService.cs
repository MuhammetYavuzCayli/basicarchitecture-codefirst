namespace BasicArchitecture.Service.Interfaces;

public interface IAuthService
{
    Task<Result<bool>> RegisterAsync(RegisterDto dto);
    Task<Result<TokenResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<TokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto dto);
    Task<Result<AccountDto>> GetUserAsync(int userId);
}
