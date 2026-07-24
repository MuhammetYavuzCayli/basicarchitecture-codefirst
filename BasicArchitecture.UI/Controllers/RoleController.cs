namespace BasicArchitecture.UI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoleController : CrudBaseController<Role, RoleDto>
{
    public RoleController(IRoleService service) : base(service)
    {
    }
}
