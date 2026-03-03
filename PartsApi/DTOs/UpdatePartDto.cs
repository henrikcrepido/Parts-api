using System.ComponentModel.DataAnnotations;
using PartsApi.Models;

namespace PartsApi.DTOs;

/// <summary>
/// DTO for updating an existing Part. All fields are optional; only supplied
/// fields will be applied (patch semantics handled in the service layer).
/// </summary>
public class UpdatePartDto
{
    [StringLength(100, MinimumLength = 1)]
    public string? PartNumber { get; set; }

    [StringLength(500, MinimumLength = 1)]
    public string? Description { get; set; }

    public UomType? Uom { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Weight must be a non-negative value.")]
    public decimal? Weight { get; set; }

    [StringLength(250)]
    public string? Demarcation { get; set; }

    public List<FlagItemDto>? MakeFlags { get; set; }

    /// <summary>
    /// When provided, replaces the entire DynamicProperties collection on the part.
    /// </summary>
    public Dictionary<string, object?>? DynamicProperties { get; set; }
}
