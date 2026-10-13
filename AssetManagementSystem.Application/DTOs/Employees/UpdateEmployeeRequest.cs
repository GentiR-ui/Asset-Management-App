namespace AssetManagementSystem.Application.DTOs.Employees;

/// <summary>
/// DepartmentId NUK eshte ketu me qellim. Levizja mes departamenteve eshte vendim
/// organizativ dhe ka endpoint-in e vet, vetem per Admin:
/// POST /api/employees/{id}/transfer-department
///
/// Pa kete fushe, nje IT-Manager nuk mund ta nxjerre punonjesin jashte scope-it te vet —
/// jo sepse nje kusht e ndalon, por sepse s'ka ku ta shkruaje. I njejti model si te asetet.
/// </summary>
public sealed record UpdateEmployeeRequest
{
    public string EmployeeCode { get; init; } = string.Empty;
}
