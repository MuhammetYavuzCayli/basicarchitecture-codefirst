namespace BasicArchitecture.Repository;

public class RangeRepository<T, TDto> : CrudRepository<T, TDto>, IRangeRepository<T, TDto> where T : class
{
    public RangeRepository(IMapper mapper, BasicArchitecturedbContext context) : base(mapper, context)
    {
    }

    public virtual async Task<List<T>> InsertRangeAsync(List<T> model)
    {
        _context.Set<T>().AddRange(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public virtual async Task<List<T>> UpdateRangeAsync(List<T> model)
    {
        _context.Set<T>().UpdateRange(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public virtual async Task<List<T>> DeleteRangeAsync(List<T> model)
    {
        _context.Set<T>().RemoveRange(model);
        await _context.SaveChangesAsync();
        return model;
    }
}
