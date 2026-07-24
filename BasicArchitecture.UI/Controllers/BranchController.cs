namespace BasicArchitecture.UI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BranchController : CrudBaseController<Branch, BranchDto>
{
    public BranchController(IBranchService service) : base(service)
    {
    }
}
