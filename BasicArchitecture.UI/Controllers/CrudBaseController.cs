namespace BasicArchitecture.UI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CrudBaseController<T, TDto> : ControllerBase
    where T : class where TDto : class
{
    protected readonly ICrudService<T, TDto> _service;

    public CrudBaseController(ICrudService<T, TDto> service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilter([FromQuery] string filter = "", [FromQuery] int skip = 0, [FromQuery] int take = 10, [FromQuery] string orderby = "")
    {
        var builder = new QueryBuilder(filter: filter, sort: orderby, page: take, pageSize: take, skip: skip);
        var queryHelper = builder.Build();
        ApplyEnforcedScope(queryHelper);
        return Ok(await _service.GetFilter(queryHelper));
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get(string id = "") => Ok(await _service.Get(id));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Insert([FromBody] TDto model) => Ok(await _service.Add(model));

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromBody] TDto model) => Ok(await _service.Update(model));

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromBody] TDto model) => Ok(await _service.Delete(model));

    // IDOR protection: the `filter` string coming from the client is entirely client-controlled
    // and can be manipulated. So, based on role, a restriction that is INDEPENDENT of the client
    // filter and always applied as an AND clause is written into QueryHelper.EnforcedExpression/
    // EnforcedValue. Single-level hierarchy: Admin/SuperAdmin are unrestricted, other roles only
    // see records within their own BranchId (no restriction is applied if the entity has no
    // BranchId — e.g. non-branch-scoped reference tables like Role).
    protected void ApplyEnforcedScope(QueryHelper queryHelper)
    {
        var roles = HttpContext.Items["UserRoles"] as List<string> ?? new List<string>();
        if (roles.Contains(Constants.Roles.Admin) || roles.Contains(Constants.Roles.SuperAdmin))
            return;

        if (typeof(T).GetProperty("BranchId") is null)
            return;

        var branchId = HttpContext.Items["BranchId"] as int?;
        queryHelper.EnforcedExpression = "BranchId == @0";
        queryHelper.EnforcedValue = branchId ?? -1; // fail closed if the claim is missing: a value that never matches
    }
}

[Route("api/[controller]")]
[ApiController]
public class BaseController<T, TDto> : ControllerBase
    where T : class where TDto : class
{
    protected readonly IBaseService<T, TDto> _service;

    public BaseController(IBaseService<T, TDto> service)
    {
        _service = service;
    }
}
