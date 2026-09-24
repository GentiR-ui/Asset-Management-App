using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Application.DTOs.Employees;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementSystem.API.Controllers;


[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ItManager}")]
[Route("api/employees")]
public sealed class EmployeeController : ApiControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    // Punonjesi krijohet te POST /api/users dhe fshihet te DELETE /api/users/{id},
    // gjithmone bashke me llogarine. Ketu mbeten vetem leximi dhe perditesimi.

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _employeeService.GetEmployeeByIdAsync(id, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PageQueryRequest request, CancellationToken cancellationToken)
    {
        var employees = await _employeeService.GetEmployeesAsync(request, cancellationToken);

        return Success(employees);
    }

    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken)
    {
        var result = await _employeeService.UpdateEmployeeAsync(id, request, cancellationToken);

        return HandleResult(result);
    }

    // Fshirja behet nga DELETE /api/users/{id}: punonjesi dhe llogaria zhduken bashke,
    // ne nje transaksion, qe te mos mbetet kurre user pa punonjes ose anasjelltas.



}
