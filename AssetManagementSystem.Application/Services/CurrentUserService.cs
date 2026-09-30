using System.Security.Claims;
using AssetManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AssetManagementSystem.Application.Services;

public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid UserId
    {
        get
        {
            var idClaim = User?.FindFirstValue(ClaimTypes.NameIdentifier) 
                          ?? User?.FindFirstValue("sub");
            return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
        }
    }

    public string Role => User?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

    public Guid? DepartmentId
    {
        get
        {
            var deptClaim = User?.FindFirstValue("department_id");
            return Guid.TryParse(deptClaim, out var id) ? id : null;
        }
    }
}
