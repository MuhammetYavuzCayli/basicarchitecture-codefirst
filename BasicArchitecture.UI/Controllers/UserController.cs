namespace BasicArchitecture.UI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : CrudBaseController<User, UserDto>
{
    public UserController(IUserService service) : base(service)
    {
    }
}
