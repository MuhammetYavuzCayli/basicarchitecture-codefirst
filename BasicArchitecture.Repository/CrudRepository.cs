namespace BasicArchitecture.Repository;

// The DI-injected context is used DIRECTLY on every call — the "open a brand-new context per
// call" pattern is intentionally NOT used here, because the parameterless constructor has no
// fallback connection string and would throw at runtime.
public class CrudRepository<T, TDto> : ICrudRepository<T, TDto> where T : class
{
    protected readonly IMapper _mapper;
    protected readonly BasicArchitecturedbContext _context;

    public CrudRepository(IMapper mapper, BasicArchitecturedbContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public virtual Task<IQueryable<T>> GetFilterAsync(QueryHelper filter)
    {
        var query = filter.Builder(_context.Set<T>().AsQueryable());
        return Task.FromResult(query);
    }

    public virtual async Task<int> GetTotalCountAsync(QueryHelper filter)
    {
        var query = filter.BuildFilterOnly(_context.Set<T>().AsQueryable());
        return await query.CountAsync();
    }

    public virtual async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate)
    {
        var query = _context.Set<T>().AsQueryable();
        if (predicate is not null)
            query = query.Where(predicate);
        return await query.FirstOrDefaultAsync();
    }

    public virtual async Task<T> InsertAsync(T model)
    {
        _context.Set<T>().Add(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public virtual async Task<T> UpdateAsync(T model)
    {
        _context.Set<T>().Update(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public virtual async Task<T> DeleteAsync(T model)
    {
        _context.Set<T>().Remove(model);
        await _context.SaveChangesAsync();
        return model;
    }
}
