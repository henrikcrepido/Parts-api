using System.ComponentModel.DataAnnotations;

namespace PartsApi.DTOs;

public class FlagItemDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public bool Value { get; set; }

    [Required]
    public DateTime ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }
}
