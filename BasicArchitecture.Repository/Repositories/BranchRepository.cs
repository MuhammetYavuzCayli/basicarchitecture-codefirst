using BasicArchitecture.Repository.Interfaces;

namespace BasicArchitecture.Repository.Repositories;

public class BranchRepository : RangeRepository<Branch, BranchDto>, IBranchRepository
{
    public BranchRepository(IMapper mapper, BasicArchitecturedbContext context) : base(mapper, context)
    {
    }
}
