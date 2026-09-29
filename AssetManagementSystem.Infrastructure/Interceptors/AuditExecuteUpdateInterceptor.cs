using System.Linq.Expressions;
using System.Security.Claims;
using AssetManagementSystem.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace AssetManagementSystem.Infrastructure.Interceptors;

public sealed class AuditExecuteUpdateInterceptor : IQueryExpressionInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditExecuteUpdateInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Expression QueryCompilationStarting(Expression query, QueryExpressionEventData eventData)
    {
        if (query is not MethodCallExpression call || 
            (call.Method.Name != "ExecuteUpdate" && call.Method.Name != "ExecuteUpdateAsync"))
        {
            return query;
        }

        var genericArgs = call.Method.GetGenericArguments();
        if (genericArgs.Length == 0) return query;

        var entityType = genericArgs[0];
        
        if (!typeof(BaseEntity).IsAssignableFrom(entityType))
        {
            return query;
        }

        if (call.Arguments.Count < 2) return query;

        var setters = StripQuotes(call.Arguments[1]) as LambdaExpression;
        if (setters is null) return query;

        var body = setters.Body;

        // 1. Shto automatikisht UpdatedAt
        if (entityType.GetProperty(nameof(BaseEntity.UpdatedAt)) != null)
        {
            body = AppendSetProperty(
                body, 
                entityType, 
                nameof(BaseEntity.UpdatedAt),
                Expression.Property(null, typeof(DateTime), nameof(DateTime.UtcNow)));
        }

        // 2. Shto automatikisht UpdatedBy per kete kerkesed (Scoped me IHttpContextAccessor)
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (Guid.TryParse(userIdClaim, out var userId) && entityType.GetProperty(nameof(BaseEntity.UpdatedBy)) != null)
        {
            body = AppendSetProperty(
                body,
                entityType,
                nameof(BaseEntity.UpdatedBy),
                Expression.Constant((Guid?)userId, typeof(Guid?)));
        }

        var newSetters = Expression.Lambda(setters.Type, body, setters.Parameters);
        return call.Update(call.Object, [call.Arguments[0], newSetters]);
    }

    private static Expression AppendSetProperty(
        Expression chain, 
        Type entityType,
        string propName, 
        Expression value)
    {
        var prop = entityType.GetProperty(propName);
        if (prop is null) return chain;

        var e = Expression.Parameter(entityType, "e");
        var selector = Expression.Lambda(Expression.Property(e, prop), e);

        var method = chain.Type.GetMethods()
            .FirstOrDefault(m => m.Name == "SetProperty" 
                                 && m.GetParameters().Length == 2
                                 && m.GetParameters()[1].ParameterType.IsGenericParameter);

        if (method is null) return chain;

        var genericMethod = method.MakeGenericMethod(prop.PropertyType);

        if (value.Type != prop.PropertyType)
        {
            value = Expression.Convert(value, prop.PropertyType);
        }

        return Expression.Call(chain, genericMethod, selector, value);
    }

    private static Expression StripQuotes(Expression e) =>
        e is UnaryExpression { NodeType: ExpressionType.Quote } u ? u.Operand : e;
}
