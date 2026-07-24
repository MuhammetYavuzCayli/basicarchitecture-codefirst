using BasicArchitecture.UI.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BasicArchitecture.UI.Extension;

// Because CrudBaseController<T,TDto> is generic, the actual type of the 200 response
// (Result<TDto> / Result<List<TDto>>) cannot be expressed in a compile-time attribute
// (CS0416). This filter inspects the action's DeclaringType at runtime and injects the
// correct closed generic Result<> schema into the Swagger document.
public class GenericResultOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var controllerType = context.MethodInfo.DeclaringType;
        if (controllerType is null || !controllerType.IsGenericType
            || controllerType.GetGenericTypeDefinition() != typeof(CrudBaseController<,>))
        {
            return;
        }

        var tDto = controllerType.GetGenericArguments()[1];
        Type? responseType = context.MethodInfo.Name switch
        {
            nameof(CrudBaseController<object, object>.GetFilter) => typeof(Result<>).MakeGenericType(typeof(List<>).MakeGenericType(tDto)),
            nameof(CrudBaseController<object, object>.Get) => typeof(Result<>).MakeGenericType(tDto),
            nameof(CrudBaseController<object, object>.Insert) => typeof(Result<>).MakeGenericType(tDto),
            nameof(CrudBaseController<object, object>.Update) => typeof(Result<>).MakeGenericType(tDto),
            nameof(CrudBaseController<object, object>.Delete) => typeof(Result<>).MakeGenericType(tDto),
            _ => null
        };

        if (responseType is null || !operation.Responses.TryGetValue("200", out var response))
            return;

        var schema = context.SchemaGenerator.GenerateSchema(responseType, context.SchemaRepository);
        response.Content["application/json"] = new OpenApiMediaType { Schema = schema };
    }
}
