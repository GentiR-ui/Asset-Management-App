using AssetManagementSystem.Application.DTOs.Assets;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssetManagementSystem.API.Controllers;

// Presja do te thote OSE — Admin OSE IT-Manager. Employee nuk krijon asete.
[Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ItManager}")]
[Route("api/assets")]
public sealed class AssetsController : ApiControllerBase
{
    private readonly IAssetService _assetService;

    public AssetsController(IAssetService assetService)
    {
        _assetService = assetService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAssetRequest request, CancellationToken cancellationToken)
    {
        var result = await _assetService.CreateAsync(request, cancellationToken);

        return HandleResult(result, StatusCodes.Status201Created);
    }

    
    [HttpGet("categories")]
    public IActionResult GetCategories() => Success(Enum.GetNames<AssetCategory>());

    /// <summary>Statuset e lejuara — po ashtu per dropdown/filtra.</summary>
    [HttpGet("statuses")]
    public IActionResult GetStatuses() => Success(Enum.GetNames<AssetStatus>());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _assetService.GetByIdAsync(id, cancellationToken);

        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var assets = await _assetService.GetAllAsync(cancellationToken);

        return Success(assets);         
    }

    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateAssetRequest request, CancellationToken cancellationToken)
    {
        var result = await _assetService.UpdateAsync(id, request, cancellationToken);

        return HandleResult(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _assetService.DeleteAsync(id, cancellationToken);

        return HandleResult(result, "Asset deleted successfully.");
    }



}
