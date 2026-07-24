using BasicArchitecture.Service.Interfaces;

namespace BasicArchitecture.Service.Services;

public class BranchService : CrudService<Branch, BranchDto>, IBranchService
{
    public BranchService(ICrudRepository<Branch, BranchDto> repository, IMapper mapper, BasicArchitecturedbContext context)
        : base(repository, mapper, context)
    {
    }
}
