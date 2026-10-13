using AssetManagementSystem.Application.Common.Authorization;
using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Application.DTOs.Employees;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;
namespace AssetManagementSystem.Application.Services;
public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly ICacheService _cacheService;
    private readonly IAssetScopeResolver _scopeResolver;
    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IAssetRepository assetRepository,
        ICacheService cacheService,
        IAssetScopeResolver scopeResolver)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _assetRepository = assetRepository;
        _cacheService = cacheService;
        _scopeResolver = scopeResolver;
    }
    public async Task<ErrorOr<EmployeeResponse>> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var scope = await _scopeResolver.ResolveScopeAsync(cancellationToken);
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return EmployeeErrors.NotFound(employeeId);
        }
        // Roja e vetme qe mbetet: punonjesi duhet te jete i departamentit tim.
        // Roja e dyte ("mos e terhiq nga nje departament tjeter") nuk duhet me —
        // DepartmentId u hoq nga UpdateEmployeeRequest, pra s'ka ku te shkruhet.
        if (!scope.IsGlobal && employee.DepartmentId != scope.DepartmentId)
        {
            return Error.Forbidden(description: "You can only manage employees in your own department.");
        }
        var employeeCode = request.EmployeeCode.Trim();
        if (await _employeeRepository.EmployeeCodeExistsAsync(employeeCode, employeeId, cancellationToken))
        {
            return EmployeeErrors.EmployeeCodeAlreadyExists(employeeCode);
        }
        employee.EmployeeCode = employeeCode;
        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        // Pa invalidim te scope-it: departamenti nuk ndryshon dot nga kjo metode.
        // Ai invalidim jeton te TransferEmployeeAsync, ku departamenti ndryshon vertet.
        var updated = await _employeeRepository.GetByIdWithDetailsAsync(employeeId, cancellationToken);
        return updated!.ToEmployeeResponse();
    }

    /// <summary>
    /// Levizja e nje punonjesi ne nje departament tjeter. Vetem Admin — pra pa roje scope-i:
    /// nje IT-Manager qe lejohej te transferonte vetem brenda departamentit te vet nuk do
    /// kishte cfare te transferonte.
    /// </summary>
    public async Task<ErrorOr<Success>> TransferEmployeeAsync(Guid employeeId, TransferEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return EmployeeErrors.NotFound(employeeId);
        }
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
        if (department is null)
        {
            return DepartmentErrors.NotFound(request.DepartmentId);
        }
        if (employee.DepartmentId == request.DepartmentId)
        {
            return Result.Success;
        }
        // Asetet kane DepartmentId-ne e vet dhe NUK levizin bashke me punonjesin.
        // Nje punonjes qe largohet duke mbajtur pajisje do linte asete te departamentit
        // te vjeter ne duart e dikujt te departamentit te ri.
        if (await _assetRepository.HasAssetsAssignedToEmployeeAsync(employeeId, cancellationToken))
        {
            return EmployeeErrors.CannotChangeDepartmentWithAssignedAssets(employeeId);
        }
        employee.DepartmentId = request.DepartmentId;
        await _employeeRepository.UpdateAsync(employee, cancellationToken);
        // Scope-i i ketij useri lexohet nga databaza me cache 60s. Pa kete rresht,
        // deri ne nje minute do te shihte ende asetet e departamentit te vjeter.
        await _cacheService.RemoveAsync(
            AssetScopeResolver.CacheKeyFor(employee.UserId), cancellationToken);
        return Result.Success;
    }
    public async Task<ErrorOr<EmployeeResponse>> GetEmployeeByIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var scope = await _scopeResolver.ResolveScopeAsync(cancellationToken);
        var employee = await _employeeRepository.GetByIdWithDetailsAsync(employeeId, cancellationToken);
        if (employee is null)
        {
            return EmployeeErrors.NotFound(employeeId);
        }
        if (!scope.IsGlobal && employee.DepartmentId != scope.DepartmentId)
        {
            return Error.Forbidden(description: "You can only view employees in your own department.");
        }
        return employee.ToEmployeeResponse();
    }
    public async Task<PagedResponse<EmployeeResponse>> GetEmployeesAsync(PageQueryRequest request, CancellationToken cancellationToken = default)
    {
        var scope = await _scopeResolver.ResolveScopeAsync(cancellationToken);
        // Filtrimi behet ne SQL, jo pas leximit: nje menaxher nuk duhet t'i marre kurre
        // rreshtat e departamenteve te tjera, as per t'i hedhur pastaj.
        var departmentId = scope.IsGlobal ? null : scope.DepartmentId;
        var page = await _employeeRepository.GetPagedAsync(request.Page, request.PageSize, departmentId, cancellationToken);
        return page.ToPagedResponse(request.Page, request.PageSize, employee => employee.ToEmployeeResponse());
    }
}
