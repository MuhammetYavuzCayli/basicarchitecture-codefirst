using BasicArchitecture.Repository.Interfaces;

namespace BasicArchitecture.Repository.Repositories;

public class UserRepository : RangeRepository<User, UserDto>, IUserRepository
{
    public UserRepository(IMapper mapper, BasicArchitecturedbContext context) : base(mapper, context)
    {
    }
}
