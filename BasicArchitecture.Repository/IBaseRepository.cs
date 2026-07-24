namespace BasicArchitecture.Repository;

public interface ICrudRepository<T, TDto>
{
    Task<IQueryable<T>> GetFilterAsync(QueryHelper filter);
    Task<int> GetTotalCountAsync(QueryHelper filter);
    Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>>? predicate);
    Task<T> InsertAsync(T model);
    Task<T> UpdateAsync(T model);
    Task<T> DeleteAsync(T model);
}

public interface IRangeRepository<T, TDto> : ICrudRepository<T, TDto>
{
    Task<List<T>> InsertRangeAsync(List<T> model);
    Task<List<T>> UpdateRangeAsync(List<T> model);
    Task<List<T>> DeleteRangeAsync(List<T> model);
}

public interface IBaseRepository<T, TDto> : IRangeRepository<T, TDto>
{
    Task<TDto> InsertDtoAsync(T model);
    Task<TDto> UpdateDtoAsync(T model);
}
