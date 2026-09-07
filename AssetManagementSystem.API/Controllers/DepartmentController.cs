using AssetManagementSystem.Application.DTOs.Department;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementSystem.API.Controllers;


[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ItManager}")]
[Route("api/departments")]
public sealed class DepartmentController : ApiControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _departmentService.CreateDepartmentAsync(request, cancellationToken);

        return HandleResult(result, StatusCodes.Status201Created);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _departmentService.GetDepartmentByIdAsync(id, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllDepartmentsAsync(cancellationToken);

        return Success(departments);         
    }

    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateDepartmentRequest request, CancellationToken cancellationToken)
    {
        var result = await _departmentService.UpdateDepartmentAsync(id, request, cancellationToken);

        return HandleResult(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _departmentService.DeleteDepartmentAsync(id, cancellationToken);

        return HandleResult(result, "Department deleted successfully.");
    }



}
