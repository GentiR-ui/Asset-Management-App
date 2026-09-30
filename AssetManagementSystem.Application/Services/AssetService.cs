using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.DTOs.Assets;
using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using AssetManagementSystem.Domain.ReadModels;
using AssetManagementSystem.Domain.Enums;
using ErrorOr;
using AssetManagementSystem.Application.Common.Caching;

namespace AssetManagementSystem.Application.Services;

public sealed class AssetService : IAssetService
{
    private readonly IAssetRepository _assetRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAssetScopeResolver _scopeResolver;
    public AssetService(
        IAssetRepository assetRepository,
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IAssetScopeResolver scopeResolver
        )
    {
        _assetRepository = assetRepository;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _scopeResolver = scopeResolver;
    }


    public async Task<ErrorOr<AssetResponse>> CreateAsync(
        CreateAssetRequest request,
        CancellationToken cancellationToken = default,
        ICacheService _cacheService = default!)
    {
        var scope = _scopeResolver.ResolveScope();

        if (!scope.IsGlobal && request.DepartmentId != scope.DepartmentId)
        {
            return Error.Forbidden(description: "You can only create assets for your assigned department.");
        }
        var assetTag = request.AssetTag.Trim();
        var serialNumber = request.SerialNumber.Trim();

        if (await _assetRepository.AssetTagExistsAsync(assetTag, cancellationToken))
        {
            return AssetErrors.AssetTagAlreadyExists(assetTag);
        }

        if (await _assetRepository.SerialNumberExistsAsync(serialNumber, cancellationToken))
        {
            return AssetErrors.SerialNumberAlreadyExists(serialNumber);
        }

        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId, cancellationToken);
        if (department is null)
        {
            return DepartmentErrors.NotFound(request.DepartmentId);
        }

        var asset = new Asset
        {
            AssetTag = assetTag,
            Name = request.Name.Trim(),
            Category = request.Category,
            SerialNumber = serialNumber,
            DepartmentId = request.DepartmentId,
            Status = AssetStatus.InStock,
            PurchaseDate = request.PurchaseDate,
            PurchasePrice = request.PurchasePrice,
            WarrantyExpiryDate = request.WarrantyExpiryDate,
            Notes = request.Notes
        };

        await _assetRepository.AddAsync(asset, cancellationToken);

        await _cacheService.InvalidateDashboardAsync(asset.DepartmentId, cancellationToken);

        return asset.ToAssetResponse();
    }

    public async Task<ErrorOr<AssetResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var scope = _scopeResolver.ResolveScope(); // <-- KËTU

        var asset = await _assetRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (asset is null)
        {
            return AssetErrors.NotFound(id);
        }

        // Refuzim nëse nuk është Admin dhe nuk i takon departamentit të tij
        if (!scope.IsGlobal && asset.DepartmentId != scope.DepartmentId)
        {
            return AssetErrors.NotFound(id); // ose Error.Forbidden
        }

        return asset.ToAssetResponse();
    }


    
    public async Task<PagedResponse<AssetResponse>> GetPagedAsync(
        AssetQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        var scope = _scopeResolver.ResolveScope();

        var filterDepartmentId = scope.IsGlobal ? null : scope.DepartmentId;
        var filter = new AssetFilter(
            request.Category, 
            request.Status, 
            request.Search, 
            request.Page, 
            request.PageSize,
            filterDepartmentId);

        

        var page = await _assetRepository.GetPagedAsync(filter, cancellationToken);

        return page.ToPagedResponse(request.Page, request.PageSize, asset => asset.ToAssetResponse());
    }

    public async Task<ErrorOr<AssetResponse>> UpdateAsync(
        Guid id,
        UpdateAssetRequest request,
        CancellationToken cancellationToken = default,
        ICacheService _cacheService = default!)
    {
        var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);

        if (asset is null)
        {
            return AssetErrors.NotFound(id);
        }

        var assetTag = request.AssetTag.Trim();
        var serialNumber = request.SerialNumber.Trim();

        if (!string.Equals(asset.AssetTag, assetTag, StringComparison.OrdinalIgnoreCase)
            && await _assetRepository.AssetTagExistsAsync(assetTag, cancellationToken))
        {
            return AssetErrors.AssetTagAlreadyExists(assetTag);
        }

        if (!string.Equals(asset.SerialNumber, serialNumber, StringComparison.OrdinalIgnoreCase)
            && await _assetRepository.SerialNumberExistsAsync(serialNumber, cancellationToken))
        {
            return AssetErrors.SerialNumberAlreadyExists(serialNumber);
        }

        asset.AssetTag = assetTag;
        asset.Name = request.Name.Trim();
        asset.SerialNumber = serialNumber;
        asset.Category = request.Category;
        asset.Status = request.Status;
        asset.PurchaseDate = request.PurchaseDate;
        asset.PurchasePrice = request.PurchasePrice;
        asset.WarrantyExpiryDate = request.WarrantyExpiryDate;
        asset.Notes = request.Notes;

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await _cacheService.InvalidateDashboardAsync(asset.DepartmentId, cancellationToken);
        // Rilexohet me Include: entiteti i mesiperm s'i ka navigimet e ngarkuara,
        // dhe vendosja e tyre para Update() do t'i shenonte Employees e Users si te ndryshuar.
        var updated = await _assetRepository.GetByIdWithDetailsAsync(id, cancellationToken);

        return updated!.ToAssetResponse();
    }

    public async Task<ErrorOr<Success>> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default,
        ICacheService _cacheService = default!)
    {
        var asset = await _assetRepository.GetByIdAsync(id, cancellationToken);

        if (asset is null)
        {
            return AssetErrors.NotFound(id);
        }

        
        await _assetRepository.RemoveAsync(asset, cancellationToken);

        await _cacheService.InvalidateDashboardAsync(asset.DepartmentId, cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> AssignAssetToEmployeeAsync(
        Guid assetId,
        AssignAssetRequest request,
        CancellationToken cancellationToken = default,
        ICacheService _cacheService = default!)
    {
        var scope = _scopeResolver.ResolveScope();

        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null) return AssetErrors.NotFound(assetId);

        if (!scope.IsGlobal && asset.DepartmentId != scope.DepartmentId)
        {
            return Error.Forbidden(description: "You can only manage assets within your department.");
        }


        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId, cancellationToken);
        if (employee is null) return EmployeeErrors.NotFound(request.EmployeeId);

        // Mbajtesi aktual, jo ai qe po kerkohet — perndryshe mesazhi tregon personin e gabuar.
        if (asset.AssignedToEmployeeId is not null)
        {
            return AssetErrors.AlreadyAssigned(assetId, asset.AssignedToEmployeeId.Value);
        }

        if (employee.DepartmentId != asset.DepartmentId)
        {
            return AssetErrors.EmployeeNotInAssetDepartment;
        }

        asset.AssignedToEmployeeId = request.EmployeeId;
        asset.Status = AssetStatus.Assigned;
        
        await _assetRepository.UpdateAsync(asset, cancellationToken);
        
        await _cacheService.InvalidateDashboardAsync(asset.DepartmentId, cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> UnassignAssetFromEmployeeAsync(
        Guid assetId,
        CancellationToken cancellationToken = default,
        ICacheService _cacheService = default!)
    {
        var scope = _scopeResolver.ResolveScope();

        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null) return AssetErrors.NotFound(assetId);

        if (!scope.IsGlobal && asset.DepartmentId != scope.DepartmentId)
        {
            return Error.Forbidden(description: "You can only manage assets within your department.");
        }

        if (asset.AssignedToEmployeeId is null) return AssetErrors.NotAssigned(assetId);

        asset.AssignedToEmployeeId = null;
        asset.Status = AssetStatus.InStock;

        await _assetRepository.UpdateAsync(asset, cancellationToken);

        await _cacheService.InvalidateDashboardAsync(asset.DepartmentId, cancellationToken);

        return Result.Success;
    }

    public async Task<ErrorOr<Success>> TransferAssetToDepartmentAsync(
        Guid assetId,
        Guid newDepartmentId,
         CancellationToken cancellationToken = default,
         ICacheService cacheService = default!)
    {
        
        var asset = await _assetRepository.GetByIdAsync(assetId, cancellationToken);
        if (asset is null) return AssetErrors.NotFound(assetId);

        

        var departmentExists = await _departmentRepository.GetByIdAsync(newDepartmentId, cancellationToken);
        if (departmentExists is null) return DepartmentErrors.NotFound(newDepartmentId);

        // Logjika e sigurisë: Nëse aseti i është caktuar një punëtori, ia heqim caktimin para transferit
        if (asset.AssignedToEmployeeId is not null)
        {
            asset.AssignedToEmployeeId = null;
            asset.Status = AssetStatus.InStock;
        }

        asset.DepartmentId = newDepartmentId;

        await _assetRepository.UpdateAsync(asset, cancellationToken);
        await cacheService.InvalidateDashboardAsync(asset.DepartmentId, cancellationToken);

        return Result.Success;
    }


    public async Task<ErrorOr<Success>> ResetDepartmentAssetsAsync(
    Guid departmentId, 
    CancellationToken cancellationToken = default, 
    ICacheService cacheService = default!)
    {
        
        var department = await _departmentRepository.GetByIdAsync(departmentId, cancellationToken);
        if (department is null)
        {
            return DepartmentErrors.NotFound(departmentId);
        }

        // Ekzekuton UPDATE-in masiv direkt ne SQL Server
        await _assetRepository.UnassignAssetsByDepartmentAsync(departmentId, cancellationToken);

        // Si ne metodat tjera te projektit tuaj, pastrojme kashen e Dashboard-it
        await cacheService.InvalidateDashboardAsync(departmentId, cancellationToken);

        return Result.Success;
    }


    
}
