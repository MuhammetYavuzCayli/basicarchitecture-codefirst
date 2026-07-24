using BasicArchitecture.Service.Interfaces;

namespace BasicArchitecture.Service.Services;

public class RoleService : CrudService<Role, RoleDto>, IRoleService
{
    public RoleService(ICrudRepository<Role, RoleDto> repository, IMapper mapper, BasicArchitecturedbContext context)
        : base(repository, mapper, context)
    {
    }
}
