using Microsoft.AspNetCore.Identity;

namespace AssetManagementSystem.Domain.Entities;


public class User : IdentityUser<Guid>
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;
    }