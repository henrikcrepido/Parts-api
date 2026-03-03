using System.Collections.Concurrent;
using PartsApi.DTOs;
using PartsApi.Models;

namespace PartsApi.Services;

public class PartService : IPartService
{
    private readonly ConcurrentDictionary<Guid, Part> _store = new();

    public Task<IEnumerable<PartDto>> GetAllAsync()
        => Task.FromResult(_store.Values.Select(MapToDto));

    public Task<PartDto?> GetByIdAsync(Guid id)
    {
        _store.TryGetValue(id, out var part);
        return Task.FromResult(part is null ? null : MapToDto(part));
    }

    public Task<PartDto> CreateAsync(CreatePartDto dto)
    {
        var part = new Part
        {
            PartNumber  = dto.PartNumber,
            Description = dto.Description,
            Uom         = dto.Uom,
            Weight      = dto.Weight,
            Demarcation = dto.Demarcation,
            MakeFlags   = dto.MakeFlags.Select(MapFlagFromDto).ToList(),
            DynamicProperties = new Dictionary<string, object?>(dto.DynamicProperties)
        };

        _store[part.Id] = part;
        return Task.FromResult(MapToDto(part));
    }

    public Task<PartDto?> UpdateAsync(Guid id, UpdatePartDto dto)
    {
        if (!_store.TryGetValue(id, out var part))
            return Task.FromResult<PartDto?>(null);

        if (dto.PartNumber  is not null) part.PartNumber  = dto.PartNumber;
        if (dto.Description is not null) part.Description = dto.Description;
        if (dto.Uom         is not null) part.Uom         = dto.Uom.Value;
        if (dto.Weight      is not null) part.Weight      = dto.Weight.Value;
        if (dto.Demarcation is not null) part.Demarcation = dto.Demarcation;
        if (dto.MakeFlags   is not null) part.MakeFlags   = dto.MakeFlags.Select(MapFlagFromDto).ToList();
        if (dto.DynamicProperties is not null)
            part.DynamicProperties = new Dictionary<string, object?>(dto.DynamicProperties);

        part.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult<PartDto?>(MapToDto(part));
    }

    public Task<bool> DeleteAsync(Guid id)
        => Task.FromResult(_store.TryRemove(id, out _));

    // ── Mapping helpers ──────────────────────────────────────────────────────

    private static PartDto MapToDto(Part part) => new()
    {
        Id          = part.Id,
        PartNumber  = part.PartNumber,
        Description = part.Description,
        Uom         = part.Uom,
        Weight      = part.Weight,
        Demarcation = part.Demarcation,
        MakeFlags   = part.MakeFlags.Select(MapFlagToDto).ToList(),
        DynamicProperties = new Dictionary<string, object?>(part.DynamicProperties),
        CreatedAt   = part.CreatedAt,
        UpdatedAt   = part.UpdatedAt
    };

    private static FlagItemDto MapFlagToDto(FlagItem f) => new()
    {
        Name      = f.Name,
        Value     = f.Value,
        ValidFrom = f.ValidFrom,
        ValidTo   = f.ValidTo
    };

    private static FlagItem MapFlagFromDto(FlagItemDto f) => new()
    {
        Name      = f.Name,
        Value     = f.Value,
        ValidFrom = f.ValidFrom,
        ValidTo   = f.ValidTo
    };
}
