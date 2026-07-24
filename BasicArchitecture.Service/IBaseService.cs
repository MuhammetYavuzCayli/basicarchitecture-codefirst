namespace BasicArchitecture.Service;

public interface ICrudService<T, TDto>
{
    Task<Result<List<TDto>>> GetFilter(QueryHelper filter);
    Task<Result<TDto>> Get(string id);
    Task<Result<TDto>> Add(TDto entity);
    Task<Result<TDto>> Update(TDto entity);
    Task<Result<bool>> Delete(TDto entity);
}

public interface IBaseService<T, TDto> : ICrudService<T, TDto> where T : class
{
    Task<Result<List<TDto>>> AddRange(List<TDto> entity);
    Task<Result<List<TDto>>> UpdateRange(List<TDto> entity);
    Task<Result<bool>> DeleteRange(List<int> ids);
}
