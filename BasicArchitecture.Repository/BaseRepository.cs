namespace BasicArchitecture.Repository;

public class BaseRepository<T, TDto> : RangeRepository<T, TDto>, IBaseRepository<T, TDto> where T : class
{
    public BaseRepository(IMapper mapper, BasicArchitecturedbContext context) : base(mapper, context)
    {
    }

    public virtual async Task<TDto> InsertDtoAsync(T model)
    {
        var inserted = await InsertAsync(model);
        return _mapper.Map<TDto>(inserted);
    }

    public virtual async Task<TDto> UpdateDtoAsync(T model)
    {
        var updated = await UpdateAsync(model);
        return _mapper.Map<TDto>(updated);
    }
}
