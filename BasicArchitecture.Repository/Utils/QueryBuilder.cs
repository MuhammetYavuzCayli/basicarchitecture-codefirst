namespace BasicArchitecture.Repository.Utils;

// Converts the raw filter/orderby/skip/take query parameters received in the controller into a QueryHelper.
public class QueryBuilder
{
    private readonly string _filter;
    private readonly string _sort;
    private readonly int _skip;
    private readonly int _pageSize;

    public QueryBuilder(string filter = "", string sort = "", int page = 0, int pageSize = 10, int skip = 0)
    {
        _filter = filter;
        _sort = sort;
        _skip = skip;
        _pageSize = pageSize;
    }

    public QueryHelper Build() => new()
    {
        Filter = _filter,
        OrderBy = _sort,
        Skip = _skip,
        Take = _pageSize
    };
}
