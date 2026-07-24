namespace BasicArchitecture.Service;

public class BaseService<T, TDto> : CrudService<T, TDto>, IBaseService<T, TDto> where T : class
{
    public BaseService(ICrudRepository<T, TDto> repository, IMapper mapper, BasicArchitecturedbContext context)
        : base(repository, mapper, context)
    {
    }

    // Placeholder — unused for now, kept as a known/intentional gap rather than a half-built feature.
    public virtual Task<Result<List<TDto>>> AddRange(List<TDto> entity) => throw new NotImplementedException();
    public virtual Task<Result<List<TDto>>> UpdateRange(List<TDto> entity) => throw new NotImplementedException();
    public virtual Task<Result<bool>> DeleteRange(List<int> ids) => throw new NotImplementedException();
}
