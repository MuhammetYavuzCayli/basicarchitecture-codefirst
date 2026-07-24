namespace BasicArchitecture.Repository.Utils;

// Converts a client-supplied "field~op~value,field2~op~value2" filter string into
// System.Linq.Dynamic.Core's parameterized Where(predicate, values) overload.
// Field names are validated via reflection against T's actual public properties, and
// values are NEVER embedded as literal text (@0, @1... placeholders + a separate values
// array) — this keeps the client-controlled filter string from being a dynamic-LINQ
// injection vector.
public static class QueryFilterParser
{
    private static readonly Dictionary<string, string> OperatorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["eq"] = "==",
        ["neq"] = "!=",
        ["gt"] = ">",
        ["gte"] = ">=",
        ["lt"] = "<",
        ["lte"] = "<=",
        ["contains"] = "Contains",
        ["startswith"] = "StartsWith",
        ["endswith"] = "EndsWith",
    };

    public static IQueryable<T> ApplyFilter<T>(IQueryable<T> source, string? filter)
    {
        /*
         
         some codes removed from here.
         
         */

        return source;
    }

    public static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> source, string? orderBy)
    {
        /*
         
        some codes removed from here.
         
         */

        return source;
    }

    private static object? ConvertValue(string raw, Type targetType)
    {
        /*
         
        some codes removed from here.
         
         */

        return null;
    }
}
