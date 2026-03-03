using System.ComponentModel.DataAnnotations;
using PartsApi.Models;

namespace PartsApi.DTOs;

/// <summary>
/// DTO for creating a new Part. Supports a fixed set of well-known properties
/// plus an open-ended DynamicProperties dictionary for any additional fields
/// required at runtime.
/// </summary>
public class CreatePartDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string PartNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(500, MinimumLength = 1)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public UomType Uom { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Weight must be a non-negative value.")]
    public decimal Weight { get; set; }

    [StringLength(250)]
    public string Demarcation { get; set; } = string.Empty;

    public List<FlagItemDto> MakeFlags { get; set; } = new();

    /// <summary>
    /// Optional free-form properties. Use this to attach any domain-specific
    /// attributes that are not covered by the fixed fields above
    /// (e.g. {"color":"red","tensileStrength":800}).
    /// </summary>
    public Dictionary<string, object?> DynamicProperties { get; set; } = new();
}
