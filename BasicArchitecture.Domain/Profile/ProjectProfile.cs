using BasicArchitecture.Core.Dtos;
using BasicArchitecture.Domain.MyDbContext;

namespace BasicArchitecture.Domain.Profile
{
    public class ProjectProfile : AutoMapper.Profile
    {
        public ProjectProfile()
        {
            CreateMap<Role, RoleDto>();
            CreateMap<Branch, BranchDto>()
                .ForMember(d => d.Users, opt => opt.Ignore());
            CreateMap<User, UserDto>();

            // Reverse direction (Dto -> Entity) is used by AutoMapper in Insert/Update flows.
            CreateMap<RoleDto, Role>();
            CreateMap<BranchDto, Branch>()
                .ForMember(d => d.Users, opt => opt.Ignore());
            CreateMap<UserDto, User>()
                .ForMember(d => d.Branch, opt => opt.Ignore())
                .ForMember(d => d.Roles, opt => opt.Ignore())
                .ForMember(d => d.PasswordHistories, opt => opt.Ignore())
                .ForMember(d => d.UserTokens, opt => opt.Ignore());
        }
    }
}
