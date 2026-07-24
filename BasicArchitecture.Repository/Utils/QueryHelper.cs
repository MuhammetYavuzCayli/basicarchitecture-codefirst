namespace BasicArchitecture.Repository.Utils;

// Query definition shared between GetFilter and GetTotalCountAsync. EnforcedExpression/
// EnforcedValue are populated by CrudBaseController.ApplyEnforcedScope for role-based IDOR
// protection — applied as an AND clause that is always enforced, independent of the client filter.
public class QueryHelper
{
    public string Filter { get; set; } = string.Empty;
    public string OrderBy { get; set; } = string.Empty;
    public int Skip { get; set; }
    public int Take { get; set; }

    public string? EnforcedExpression { get; set; }
    public object? EnforcedValue { get; set; }

    // Filter + enforced scope + orderby + skip/take (for a normal list query).
    public IQueryable<T> Builder<T>(IQueryable<T> source)
    {
        source = BuildFilterOnly(source);
        if (Skip > 0) source = source.Skip(Skip);
        if (Take > 0) source = source.Take(Take);
        return source;
    }

    // Filter + enforced scope (no skip/take) — kept separate so GetTotalCountAsync returns
    // the true total count, unbounded by the page size.
    public IQueryable<T> BuildFilterOnly<T>(IQueryable<T> source)
    {
        /*
         
         Some codes removed from here.
         
         */
        return source;
    }
}
