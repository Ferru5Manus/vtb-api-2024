using System.ComponentModel.DataAnnotations;
using UserService.Attributes.Validation;

namespace UserService.Models.Requests;

/// <summary>
/// Запрос на регистрацию пользователя.
/// </summary>
public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    // TODO: Add validation
    [Required]
    [ValidUsername]
    public string Username { get; set; } = null!;

    // TODO: Add validation
    [Required]
    [ValidPassword]
    public string Password { get; set; } = null!;
}