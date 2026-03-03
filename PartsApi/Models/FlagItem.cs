namespace PartsApi.Models;

public class FlagItem
{
    public string Name { get; set; } = string.Empty;
    public bool Value { get; set; }
    public DateTime ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public bool IsValid(DateTime? atDate = null)
    {
        var checkDate = atDate ?? DateTime.UtcNow;
        return checkDate >= ValidFrom && (ValidTo == null || checkDate <= ValidTo);
    }
}
