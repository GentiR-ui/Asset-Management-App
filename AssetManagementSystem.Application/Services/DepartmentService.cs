using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.DTOs.Department;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public DepartmentService(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository)
    {
        _departmentRepository = departmentRepository;
        _employeeRepository = employeeRepository;
    }

    
    public async Task<ErrorOr<DepartmentResponse>> CreateDepartmentAsync(CreateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var department = new Domain.Entities.Department
        {
            Name = request.Name,
            Description = request.Description,
            Code = request.Code
        };

        if(await _departmentRepository.NameExistsAsync(request.Name, null, cancellationToken))
        {
            return DepartmentErrors.NameAlreadyExists(request.Name);
        }

        if(await _departmentRepository.CodeExistsAsync(request.Code, null, cancellationToken))
        {
            return DepartmentErrors.CodeAlreadyExists(request.Code);
        }

        await _departmentRepository.AddAsync(department, cancellationToken);

        return department.ToDepartmentResponse();
    }
    public async Task<ErrorOr<DepartmentResponse>> UpdateDepartmentAsync(Guid departmentId, UpdateDepartmentRequest request, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken);


        if(department == null)
        {
            return DepartmentErrors.NotFound(departmentId);

        }

        if(await _departmentRepository.NameExistsAsync(request.Name, departmentId, cancellationToken))
        {
            return DepartmentErrors.NameAlreadyExists(request.Name);
        }

        if(await _departmentRepository.CodeExistsAsync(request.Code, departmentId, cancellationToken))
        {
            return DepartmentErrors.CodeAlreadyExists(request.Code);
        }

        department.Name = request.Name;
        department.Description = request.Description;
        department.Code = request.Code;

        await _departmentRepository.UpdateAsync(department, cancellationToken);

        return department.ToDepartmentResponse();
    }

        public async Task<ErrorOr<DepartmentResponse>> GetDepartmentByIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken);

        if(department == null)
        {
            return DepartmentErrors.NotFound(departmentId);
        }

        return department.ToDepartmentResponse();
    }

    public async Task<IReadOnlyList<DepartmentResponse>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default)
    {
         var departments = await _departmentRepository.GetAllAsync(cancellationToken);
        return departments.Select(d => d.ToDepartmentResponse()).ToList();
    }
    public async Task<ErrorOr<Success>> DeleteDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken);

        if(department == null)
        {
            return DepartmentErrors.NotFound(departmentId);
        }

        // FK_Employees_Departments_DepartmentId eshte Restrict.
        if (await _employeeRepository.HasEmployeesInDepartmentAsync(departmentId, cancellationToken))
        {
            return DepartmentErrors.CannotDeleteWithEmployees(departmentId);
        }

        await _departmentRepository.RemoveAsync(department, cancellationToken);

        return Result.Success;
    }


        
}