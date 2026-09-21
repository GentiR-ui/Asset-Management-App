using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementSystem.API.Controllers;

[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ItManager}")]
[Route("api/dashboard")]
public sealed class DashboardController : ApiControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("total-value")]
    public async Task<IActionResult> GetTotalValue(CancellationToken cancellationToken) =>
        Success(await _dashboardService.GetAssetValueSummaryAsync(cancellationToken));

    [HttpGet("value-by-department")]
    public async Task<IActionResult> GetValueByDepartment(CancellationToken cancellationToken) =>
        Success(await _dashboardService.GetValueByDepartmentAsync(cancellationToken));

    [HttpGet("assets-by-status")]
    public async Task<IActionResult> GetAssetsByStatus(CancellationToken cancellationToken) =>
        Success(await _dashboardService.GetAssetsByStatusAsync(cancellationToken));

    [HttpGet("assets-by-category")]
    public async Task<IActionResult> GetAssetsByCategory(CancellationToken cancellationToken) =>
        Success(await _dashboardService.GetAssetsByCategoryAsync(cancellationToken));

    [HttpGet("asset-age")]
    public async Task<IActionResult> GetAssetAge(CancellationToken cancellationToken) =>
        Success(await _dashboardService.GetAssetAgeDistributionAsync(cancellationToken));
}
