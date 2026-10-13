using AssetManagementSystem.Domain.Common;
using Microsoft.AspNetCore.Identity;

namespace AssetManagementSystem.Domain.Entities;

public class User : IdentityUser<Guid>, IAuditable
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // Te katra nullable, per dy arsye te ndryshme:
    //   CreatedBy  — /auth/register krijon user kur askush nuk eshte i autentikuar
    //   CreatedAt  — userat ekzistues u krijuan para se kolonat te ekzistonin
    // Nje Guid jo-nullable do e rrezonte login-in, si ndodhi me BaseEntity.CreatedBy.
    public DateTime? CreatedAt { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }
}
