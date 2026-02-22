using System.ComponentModel.DataAnnotations;

namespace Challenge.Infrastructure.Auth;

public class JwtSettings
{
    [Required(ErrorMessage = "JWT key is required.")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "JWT issuer is required.")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "JWT audience is required.")]
    public string Audience { get; set; } = string.Empty;

    [Required(ErrorMessage = "JWT expiration time is required.")]
    [Range(1, int.MaxValue, ErrorMessage = "JWT expiration time must be greater than 0.")]
    public int ExpireMinutes { get; set; }
}