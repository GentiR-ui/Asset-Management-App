using AssetManagementSystem.Application.Common.Mappings;
using AssetManagementSystem.Application.DTOs.Common;
using AssetManagementSystem.Application.DTOs.Users;
using AssetManagementSystem.Application.Interfaces;
using AssetManagementSystem.Domain.Common;
using AssetManagementSystem.Domain.Entities;
using AssetManagementSystem.Domain.Errors;
using AssetManagementSystem.Domain.Interfaces;
using ErrorOr;

namespace AssetManagementSystem.Application.Services;

public class UserService : IUserService
{
    private readonly IIdentityProvider _identityProvider;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAssetRepository _assetRepository;
    private readonly ITransactionRunner _transactionRunner;

    public UserService(IIdentityProvider identityProvider, IEmployeeRepository employeeRepository, IDepartmentRepository departmentRepository, IAssetRepository assetRepository, ITransactionRunner transactionRunner)
    {
        _identityProvider = identityProvider;
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _assetRepository = assetRepository;
        _transactionRunner = transactionRunner;
    }

    public async Task<ErrorOr<UserResponse>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        // Liste e qarte, jo "gjithcka pervec Admin": ajo forme prishet sa here shtohet nje rol i ri.
        var needsEmployee = AppRoles.Staff.Contains(request.Role);
        var employeeCode = request.EmployeeCode?.Trim();

        // Kontrolle paraprake: japin mesazh te qarte para se te hapet transaksioni.
        if (needsEmployee)
        {
            if (await _departmentRepository.GetByIdAsync(request.DepartmentId!.Value, cancellationToken) is null)
            {
                return DepartmentErrors.NotFound(request.DepartmentId.Value);
            }

            if (await _employeeRepository.EmployeeCodeExistsAsync(employeeCode!, null, cancellationToken))
            {
                return EmployeeErrors.EmployeeCodeAlreadyExists(employeeCode!);
            }
        }

        // Dy shkrime ne dy tabela: ose te dyja, ose asnjera.
        return await _transactionRunner.ExecuteAsync<UserResponse>(async () =>
        {
            var created = await _identityProvider.CreateUserAsync(
                request.FirstName, request.LastName, request.Email, request.Password, emailConfirmed: true);

            if (created.IsError)
            {
                return created.Errors;
            }

            var user = created.Value;

            // AssignRoleAsync dhe jo AddToRoleAsync: kjo kthen ErrorOr, pra deshtimi shkakton rollback.
            var roleResult = await _identityProvider.AssignRoleAsync(user, request.Role);

            if (roleResult.IsError)
            {
                return roleResult.Errors;
            }

            if (needsEmployee)
            {
                await _employeeRepository.AddAsync(new Employee
                {
                    EmployeeCode = employeeCode!,
                    UserId = user.Id,
                    DepartmentId = request.DepartmentId!.Value
                }, cancellationToken);
            }

            return user.ToUserResponse([request.Role]);
        }, cancellationToken);
    }

    public async Task<PagedResponse<UserResponse>> GetUsersAsync(PageQueryRequest request)
    {
        var page = await _identityProvider.GetUsersPagedAsync(request.Page, request.PageSize);
        var userResponses = new List<UserResponse>();

        // TODO: N+1 — nje query per cdo user. Faqosja e zbut (vetem nje faqe eshte ne loop),
        // por zgjidhja e vertete eshte nje JOIN i vetem mbi UserRoles, ne Fazen 2.
        foreach (var user in page.Items)
        {
            var roles = await _identityProvider.GetRolesAsync(user);
            userResponses.Add(user.ToUserResponse(roles));
        }

        return new PagedResponse<UserResponse>
        {
            Items = userResponses,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = page.TotalCount
        };
    }

    public async Task<ErrorOr<Success>> AssignRoleAsync(Guid userId, AssignRoleRequest request)
    {
        var user = await _identityProvider.FindByIdAsync(userId);

        if (user is null)
        {
            return UserErrors.NotFound(userId);
        }

        return await _identityProvider.AssignRoleAsync(user, request.RoleName);
    }

    public async Task<ErrorOr<Success>> RemoveRoleAsync(Guid userId, string roleName)
    {
        var user = await _identityProvider.FindByIdAsync(userId);

        if (user is null)
        {
            return UserErrors.NotFound(userId);
        }

        if (await IsLastAdminAsync(user, roleName))
        {
            return UserErrors.CannotRemoveLastAdmin;
        }

        return await _identityProvider.RemoveRoleAsync(user, roleName);
    }

   

    public async Task<ErrorOr<Success>> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var user = await _identityProvider.FindByIdAsync(userId);

        if (user is null)
        {
            return UserErrors.NotFound(userId);
        }

        return await _identityProvider.UpdateUserAsync(
            user, request.FirstName, request.LastName);
    }

    public async Task<ErrorOr<Success>> DeleteUserAsync(Guid userId)
    {
        var user = await _identityProvider.FindByIdAsync(userId);
        if (user is null) return UserErrors.NotFound(userId);

        if (await IsLastAdminAsync(user, AppRoles.Admin)) return UserErrors.CannotDeleteLastAdmin;

        var employee = await _employeeRepository.GetByUserIdAsync(userId);

        if (employee is not null && await _assetRepository.HasAssetsAssignedToEmployeeAsync(employee.Id))
            return EmployeeErrors.CannotDeleteWithAssignedAssets(employee.Id);

        return await _transactionRunner.ExecuteAsync<Success>(async () =>
        {
            if (employee is not null)
                await _employeeRepository.RemoveAsync(employee);     // I PARI — FK Restrict

            return await _identityProvider.DeleteUserAsync(user);
        });

    }

    /// <summary>
    /// A eshte ky useri i FUNDIT qe e mban rolin Admin?
    /// Nese po, heqja e rolit ose fshirja e tij do ta linte sistemin pa asnje administrator —
    /// dhe askush s'do te mund ta rregullonte nga vete aplikacioni.
    /// </summary>
    private async Task<bool> IsLastAdminAsync(Domain.Entities.User user, string roleName)
    {
        if (!string.Equals(roleName, AppRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var roles = await _identityProvider.GetRolesAsync(user);

        if (!roles.Contains(AppRoles.Admin))
        {
            return false;
        }

        var adminCount = await _identityProvider.CountUsersInRoleAsync(AppRoles.Admin);

        return adminCount <= 1;
    }
}
