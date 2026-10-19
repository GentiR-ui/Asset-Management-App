namespace AssetManagementSystem.Application.DTOs.Employees;
public sealed record EmployeeDto
{
    public required Guid Id { get; init; }
    public required string EmployeeCode { get; init; }
    /// <summary>Te tria fushat e meposhtme vijne nga llogaria e lidhur, jo nga tabela Employees.</summary>
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    
}


