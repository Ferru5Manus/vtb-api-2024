using System.ComponentModel.DataAnnotations;

namespace AuthService.Services.Models;

/// <summary>
/// Представляет объект пользователя.
/// </summary>
public class UserAccessData
{
    [Required]
    public long Id { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Role { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;

    [Required]
    public string Salt { get; set; } = null!;
}