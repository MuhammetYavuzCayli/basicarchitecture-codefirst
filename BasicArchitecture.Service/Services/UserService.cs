using BasicArchitecture.Service.Interfaces;

namespace BasicArchitecture.Service.Services;

public class UserService : CrudService<User, UserDto>, IUserService
{
    public UserService(ICrudRepository<User, UserDto> repository, IMapper mapper, BasicArchitecturedbContext context)
        : base(repository, mapper, context)
    {
    }
}
