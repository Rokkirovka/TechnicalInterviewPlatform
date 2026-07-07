using System.ComponentModel.DataAnnotations;

namespace Api.Endpoints;

/// <summary>
/// Request model for user authentication.
/// </summary>
public sealed record LoginRequest
{
    /// <summary>
    /// Login of user witch given by company
    /// </summary>
    /// <example>johnsina</example>
    [Required]
    public string Login { get; init; } = string.Empty;

    /// <summary>
    /// Password for authentication witch given by company
    /// </summary>
    /// <example>MyGreaterThan8SymbolPassword</example>
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}