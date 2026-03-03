namespace PartsApi.Models;

public class Part
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PartNumber { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public UomType Uom { get; set; }
    public decimal Weight { get; set; }
    public string Demarcation { get; set; } = string.Empty;
    public List<FlagItem> MakeFlags { get; set; } = new();
    public Dictionary<string, object?> DynamicProperties { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
