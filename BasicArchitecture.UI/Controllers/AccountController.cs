namespace BasicArchitecture.UI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IAuthService _service) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _service.RegisterAsync(dto);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<TokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _service.LoginAsync(dto);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<TokenResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        var result = await _service.RefreshTokenAsync(dto);
        return result.IsSuccess ? Ok(result) : Unauthorized(result);
    }

    [HttpGet("info")]
    [Authorize]
    [ProducesResponseType(typeof(Result<AccountDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Info()
    {
        var userId = HttpContext.Items["UserId"] as int?;
        if (userId is null)
            return NotFound();

        var result = await _service.GetUserAsync(userId.Value);
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }
}
