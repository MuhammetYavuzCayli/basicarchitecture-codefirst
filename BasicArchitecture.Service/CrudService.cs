namespace BasicArchitecture.Service;

public class CrudService<T, TDto> : ICrudService<T, TDto> where T : class
{
    protected readonly ICrudRepository<T, TDto> _repository;
    protected readonly IMapper _mapper;
    protected readonly BasicArchitecturedbContext _context;

    public CrudService(ICrudRepository<T, TDto> repository, IMapper mapper, BasicArchitecturedbContext context)
    {
        _repository = repository;
        _mapper = mapper;
        _context = context;
    }

    public virtual async Task<Result<List<TDto>>> GetFilter(QueryHelper filter)
    {
        var query = await _repository.GetFilterAsync(filter);
        var list = await query.ToListAsync();
        var total = await _repository.GetTotalCountAsync(filter);
        var dtoList = _mapper.Map<List<TDto>>(list);

        var pageCount = filter.Take > 0 ? (int)Math.Ceiling(total / (double)filter.Take) : 1;

        return new Result<List<TDto>>(true, ResultTypeEnum.Success, dtoList, pageCount, total, string.Empty)
        {
            IsLastPackage = filter.Take <= 0 || filter.Skip + dtoList.Count >= total
        };
    }

    public virtual async Task<Result<TDto>> Get(string id)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty is null)
            return new Result<TDto>(false, ResultTypeEnum.Error, default!, "Entity does not have an 'Id' property.");

        object convertedId;
        try
        {
            convertedId = idProperty.PropertyType == typeof(Guid)
                ? Guid.Parse(id)
                : Convert.ChangeType(id, idProperty.PropertyType);
        }
        catch
        {
            return new Result<TDto>(false, ResultTypeEnum.Error, default!, "Invalid id.");
        }

        var parameter = Expression.Parameter(typeof(T), "s");
        var property = Expression.Property(parameter, idProperty);
        var constant = Expression.Constant(convertedId, idProperty.PropertyType);
        var predicate = Expression.Lambda<Func<T, bool>>(Expression.Equal(property, constant), parameter);

        var entity = await _repository.GetFirstOrDefaultAsync(predicate);
        if (entity is null)
            return new Result<TDto>(false, ResultTypeEnum.NotFound, default!, "Record not found.");

        return new Result<TDto>(true, ResultTypeEnum.Success, _mapper.Map<TDto>(entity), string.Empty);
    }

    public virtual async Task<Result<TDto>> Add(TDto entity)
    {
        var model = _mapper.Map<T>(entity);
        var inserted = await _repository.InsertAsync(model);
        return new Result<TDto>(true, ResultTypeEnum.Success, _mapper.Map<TDto>(inserted), string.Empty);
    }

    public virtual async Task<Result<TDto>> Update(TDto entity)
    {
        var model = _mapper.Map<T>(entity);
        var updated = await _repository.UpdateAsync(model);
        return new Result<TDto>(true, ResultTypeEnum.Success, _mapper.Map<TDto>(updated), string.Empty);
    }

    public virtual async Task<Result<bool>> Delete(TDto entity)
    {
        var model = _mapper.Map<T>(entity);
        await _repository.DeleteAsync(model);
        return new Result<bool>(true, ResultTypeEnum.Success, true, string.Empty);
    }
}
