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

        // Roja 1: punonjesi duhet te jete i departamentit tim.
        if (!scope.IsGlobal && employee.DepartmentId != scope.DepartmentId)
        {
            return Error.Forbidden(description: "You can only manage employees in your own department.");
        }

        // Roja 2: dhe nuk mund ta terheq nje punonjes te nje departamenti tjeter te vetja.
        if (!scope.IsGlobal && request.DepartmentId != scope.DepartmentId)
        {
            return Error.Forbidden(description: "You cannot move an employee to another department.");
        }

        var employeeCode = request.EmployeeCode.Trim();

        if (await _employeeRepository.EmployeeCodeExistsAsync(employeeCode, employeeId, cancellationToken))
        {
            return EmployeeErrors.EmployeeCodeAlreadyExists(employeeCode);
        }

        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);

        if (department is null)
        {
            return DepartmentErrors.NotFound(request.DepartmentId);
        }

        // Vetem nese departamenti NDRYSHON: perndryshe query-ja e aseteve behet kot.
        // Asetet kane DepartmentId-ne e vet, pra do te mbeteshin te departamenti i vjeter.
        if (employee.DepartmentId != request.DepartmentId
            && await _assetRepository.HasAssetsAssignedToEmployeeAsync(employeeId, cancellationToken))
        {
            return EmployeeErrors.CannotChangeDepartmentWithAssignedAssets(employeeId);
        }

        employee.EmployeeCode = employeeCode;
        employee.DepartmentId = request.DepartmentId;

        await _employeeRepository.UpdateAsync(employee, cancellationToken);

        // Scope-i i ketij useri lexohet nga databaza me cache 60s. Pa kete rresht,
        // deri ne nje minute do te shihte ende asetet e departamentit te vjeter.
        // Dashboard-i nuk invalidohet: me rregullin me siper, departamenti ndryshon
        // vetem kur punonjesi s'ka asete, pra asnje numer nuk preket.
        await _cacheService.RemoveAsync(
            AssetScopeResolver.CacheKeyFor(employee.UserId), cancellationToken);

        var updated = await _employeeRepository.GetByIdWithDetailsAsync(employeeId, cancellationToken);



        return updated!.ToEmployeeResponse();
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
