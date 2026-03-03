using PartsApi.Models;

namespace PartsApi.DTOs;

/// <summary>
/// Read-only DTO returned by the API for a Part resource.
/// </summary>
public class PartDto
{
    public Guid Id { get; set; }
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UomType Uom { get; set; }
    public decimal Weight { get; set; }
    public string Demarcation { get; set; } = string.Empty;
    public List<FlagItemDto> MakeFlags { get; set; } = new();

    /// <summary>
    /// Free-form properties stored alongside the standard fields.
    /// </summary>
    public Dictionary<string, object?> DynamicProperties { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
