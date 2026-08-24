using AssetManagementSystem.Application.DTOs.Users;
using AssetManagementSystem.Domain.Entities;

namespace AssetManagementSystem.Application.Common.Mappings;

public static class UserMappings
{
    public static UserResponse ToUserResponse(this User u, IEnumerable<string> roles) => new()
    {
        Id = u.Id,
        Email = u.Email ?? string.Empty,
        FirstName = u.FirstName, 
        LastName = u.LastName,
        IsEmailConfirmed = u.EmailConfirmed,
        Roles = roles.ToList()
    };
}