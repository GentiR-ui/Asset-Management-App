using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.DTOs.Employees;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly IIdentityProvider _identityProvider;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IAssetRepository assetRepository,
        IIdentityProvider identityProvider)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _assetRepository = assetRepository;
        _identityProvider = identityProvider;
    }

    public async Task<ErrorOr<EmployeeResponse>> CreateEmployeeAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _identityProvider.FindByIdAsync(request.UserId);

        if (user is null)
        {
            return EmployeeErrors.UserNotFound(request.UserId);
        }

        if (await _employeeRepository.IsUserLinkedAsync(request.UserId, cancellationToken))
        {
            return EmployeeErrors.UserAlreadyLinked(request.UserId);
        }

        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);

        if (department is null)
        {
            return DepartmentErrors.NotFound(request.DepartmentId);
        }

        var employeeCode = request.EmployeeCode.Trim();

        if (await _employeeRepository.EmployeeCodeExistsAsync(employeeCode, null, cancellationToken))
        {
            return EmployeeErrors.EmployeeCodeAlreadyExists(employeeCode);
        }

        // Navigimet vendosen drejtperdrejt: EF i nxjerr UserId dhe DepartmentId prej tyre,
        // dhe mapimi i gjen te gatshme pa nevoje per nje lexim te dyte.
        var employee = new Employee
        {
            EmployeeCode = employeeCode,
            User = user,
            Department = department
        };

        await _employeeRepository.AddAsync(employee, cancellationToken);

        return employee.ToEmployeeResponse();
    }

    public async Task<ErrorOr<EmployeeResponse>> UpdateEmployeeAsync(Guid employeeId, UpdateEmployeeRequest request, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound(employeeId);
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

        employee.EmployeeCode = employeeCode;
        employee.DepartmentId = request.DepartmentId;

        await _employeeRepository.UpdateAsync(employee, cancellationToken);

        // Rilexohet me Include: entiteti i mesiperm s'i ka navigimet e ngarkuara,
        // dhe vendosja e tyre para Update() do t'i shenonte Users e Departments si te ndryshuar.
        var updated = await _employeeRepository.GetByIdWithDetailsAsync(employeeId, cancellationToken);

        return updated!.ToEmployeeResponse();
    }

    public async Task<ErrorOr<Success>> DeleteEmployeeAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound(employeeId);
        }

        if (await _assetRepository.HasAssetsAssignedToEmployeeAsync(employeeId, cancellationToken))
        {
            return EmployeeErrors.CannotDeleteWithAssignedAssets(employeeId);
        }

        await _employeeRepository.RemoveAsync(employee, cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<EmployeeResponse>> GetEmployeeByIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        var employee = await _employeeRepository.GetByIdWithDetailsAsync(employeeId, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound(employeeId);
        }

        return employee.ToEmployeeResponse();
    }

    public async Task<IReadOnlyList<EmployeeResponse>> GetAllEmployeesAsync(CancellationToken cancellationToken = default)
    {
        var employees = await _employeeRepository.GetAllAsync(cancellationToken);

        return employees.Select(employee => employee.ToEmployeeResponse()).ToList();
    }
}
