using BasicArchitecture.Repository.Interfaces;

namespace BasicArchitecture.Repository.Repositories;

public class RoleRepository : RangeRepository<Role, RoleDto>, IRoleRepository
{
    public RoleRepository(IMapper mapper, BasicArchitecturedbContext context) : base(mapper, context)
    {
    }
}
