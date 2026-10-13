using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace AssetManagementSystem.Infrastructure.Interceptors;

/// <summary>
/// Shton UpdatedAt dhe UpdatedBy ne cdo ExecuteUpdate mbi nje BaseEntity.
///
/// Ekziston sepse ExecuteUpdate nuk kalon nga ChangeTracker-i, pra ApplyAuditFields()
/// nuk thirret kurre per te. Pa kete interceptor, rruga me e shpejte e shkrimit do te
/// ishte e vetmja pa audit.
/// </summary>
public sealed class AuditExecuteUpdateInterceptor : IQueryExpressionInterceptor
{
    private readonly CurrentUserAccessor _currentUser;
    private readonly ILogger<AuditExecuteUpdateInterceptor> _logger;

    public AuditExecuteUpdateInterceptor(
        CurrentUserAccessor currentUser,
        ILogger<AuditExecuteUpdateInterceptor> logger)
    {
        _currentUser = currentUser;
        _logger = logger;
    }

    public Expression QueryCompilationStarting(Expression query, QueryExpressionEventData eventData)
    {
        if (query is not MethodCallExpression call
            || (call.Method.Name != "ExecuteUpdate" && call.Method.Name != "ExecuteUpdateAsync"))
        {
            return query;
        }

        var genericArguments = call.Method.GetGenericArguments();
        if (genericArguments.Length == 0)
        {
            return query;
        }

        var entityType = genericArguments[0];
        if (!typeof(BaseEntity).IsAssignableFrom(entityType))
        {
            return query;
        }

        if (call.Arguments.Count < 2)
        {
            return query;
        }

        // ── Forma e setters-ave ndryshoi me EF Core 10 ───────────────────────────
        // Deri ne EF 9 vinin si LambdaExpression me zinxhir:
        //     setter => setter.SetProperty(a => a.X, v).SetProperty(a => a.Y, w)
        //
        // Ne EF 10 i njejti kod C# perkthehet ne nje VARG:
        //     new ITuple[] { new Tuple<Delegate, object>(a => a.X, (object)v), ... }
        //
        // Versioni i meparshem kerkonte lambda-n, nuk e gjente, dhe hiqte dore pa asnje
        // mesazh — prandaj nje reset i 3,332 rreshtave doli pa asnje fushe auditi.
        // Per kete, cdo forme e panjohur tani logohet si Error, nuk heshtet.
        if (StripQuotes(call.Arguments[1]) is not NewArrayExpression setterArray)
        {
            _logger.LogError(
                "AUDIT: forma e setters-ave nuk njihet ({Type}). UpdatedAt/UpdatedBy NUK u shtuan "
                + "per {Entity} — ka gjasa qe versioni i EF Core-it te kete ndryshuar.",
                call.Arguments[1].GetType().Name,
                entityType.Name);

            return query;
        }

        var setters = setterArray.Expressions.ToList();
        var originalCount = setters.Count;
        var tupleConstructor = typeof(Tuple<Delegate, object>).GetConstructors()[0];

        AppendSetter(setters, tupleConstructor, entityType,
            nameof(BaseEntity.UpdatedAt),
            Expression.Constant((DateTime?)DateTime.UtcNow, typeof(DateTime?)));

        var currentUserId = _currentUser.UserId;

        // Guid.Empty = pa kerkese HTTP (migrim, seed, background job). NULL e thote te verteten.
        if (currentUserId != Guid.Empty)
        {
            AppendSetter(setters, tupleConstructor, entityType,
                nameof(BaseEntity.UpdatedBy),
                Expression.Constant((Guid?)currentUserId, typeof(Guid?)));
        }

        if (setters.Count == originalCount)
        {
            return query;
        }

        var arguments = call.Arguments.ToList();
        arguments[1] = Expression.NewArrayInit(typeof(ITuple), setters);

        _logger.LogInformation(
            "AUDIT: {Entity} — setters {Before} -> {After}.",
            entityType.Name, originalCount, setters.Count);

        return call.Update(call.Object, arguments);
    }

    /// <summary>
    /// Shton nje element Tuple&lt;Delegate, object&gt;(a =&gt; a.Prop, (object)vlera),
    /// pikerisht forma qe EF Core 10 pret ne vargun e setters-ave.
    /// </summary>
    private static void AppendSetter(
        List<Expression> setters,
        ConstructorInfo tupleConstructor,
        Type entityType,
        string propertyName,
        Expression value)
    {
        var property = entityType.GetProperty(propertyName);
        if (property is null)
        {
            return;
        }

        // Parameter i vecante per cdo lambda — secila eshte shprehje e pavarur.
        var parameter = Expression.Parameter(entityType, "a");
        var selector = Expression.Lambda(Expression.Property(parameter, property), parameter);

        setters.Add(Expression.New(
            tupleConstructor,
            selector,
            Expression.Convert(value, typeof(object))));
    }

    private static Expression StripQuotes(Expression expression) =>
        expression is UnaryExpression { NodeType: ExpressionType.Quote } unary
            ? unary.Operand
            : expression;
}
