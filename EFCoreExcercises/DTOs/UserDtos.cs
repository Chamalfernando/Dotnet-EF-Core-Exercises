using System.ComponentModel.DataAnnotations;

namespace EFCoreExcercises.DTOs;

// Returned to clients (never expose the entity directly).
public record UserDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

// Incoming payload for POST /api/users
public class CreateUserDto
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;
}

// Incoming payload for PUT /api/users/{id}
public class UpdateUserDto
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(256)]
    public string Email { get; set; } = string.Empty;
}
