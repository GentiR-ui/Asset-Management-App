namespace AssetManagementSystem.Domain.Interfaces;
/// <summary>
/// Mban scope-in e kerkeses aktuale. Vendoset nje here nga middleware-i, lexohet
/// nga ApplicationDbContext per global query filter.
///
/// Ekziston sepse DbContext-i nuk mund te presë: zgjidhja e scope-it kerkon nje
/// lexim asinkron nga databaza, ndersa filtri duhet te jete gati para query-se se pare.
/// </summary>
public interface IScopeContext
{
    bool IsGlobal { get; }
    Guid? DepartmentId { get; }
    void SetGlobal();
    void SetDepartment(Guid departmentId);
}
