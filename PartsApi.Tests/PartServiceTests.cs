using PartsApi.DTOs;
using PartsApi.Models;
using PartsApi.Services;

namespace PartsApi.Tests;

public class PartServiceTests
{
    private static CreatePartDto BuildCreateDto(string partNumber = "P-001") => new()
    {
        PartNumber  = partNumber,
        Description = "Test part",
        Uom         = UomType.Each,
        Weight      = 1.5m,
        Demarcation = "Zone-A",
        MakeFlags   = new List<FlagItemDto>
        {
            new() { Name = "IsMake", Value = true, ValidFrom = DateTime.UtcNow, ValidTo = null }
        },
        DynamicProperties = new Dictionary<string, object?> { ["color"] = "red" }
    };

    // ── Create ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_ReturnsPartWithNewId()
    {
        var svc  = new PartService();
        var part = await svc.CreateAsync(BuildCreateDto());

        Assert.NotEqual(Guid.Empty, part.Id);
        Assert.Equal("P-001", part.PartNumber);
        Assert.Equal(UomType.Each, part.Uom);
        Assert.Equal(1.5m, part.Weight);
        Assert.Equal("Zone-A", part.Demarcation);
    }

    [Fact]
    public async Task CreateAsync_StoresMakeFlags()
    {
        var svc  = new PartService();
        var part = await svc.CreateAsync(BuildCreateDto());

        var flag = Assert.Single(part.MakeFlags);
        Assert.Equal("IsMake", flag.Name);
        Assert.True(flag.Value);
    }

    [Fact]
    public async Task CreateAsync_StoresDynamicProperties()
    {
        var svc  = new PartService();
        var part = await svc.CreateAsync(BuildCreateDto());

        Assert.True(part.DynamicProperties.ContainsKey("color"));
        Assert.Equal("red", part.DynamicProperties["color"]?.ToString());
    }

    // ── GetAll ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_ReturnsAllCreatedParts()
    {
        var svc = new PartService();
        await svc.CreateAsync(BuildCreateDto("P-001"));
        await svc.CreateAsync(BuildCreateDto("P-002"));

        var all = (await svc.GetAllAsync()).ToList();
        Assert.Equal(2, all.Count);
    }

    // ── GetById ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectPart()
    {
        var svc     = new PartService();
        var created = await svc.CreateAsync(BuildCreateDto());

        var fetched = await svc.GetByIdAsync(created.Id);
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullForUnknownId()
    {
        var svc = new PartService();
        var result = await svc.GetByIdAsync(Guid.NewGuid());
        Assert.Null(result);
    }

    // ── Update ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task UpdateAsync_AppliesChanges()
    {
        var svc     = new PartService();
        var created = await svc.CreateAsync(BuildCreateDto());

        var updated = await svc.UpdateAsync(created.Id, new UpdatePartDto
        {
            Description = "Updated description",
            Weight      = 3.0m
        });

        Assert.NotNull(updated);
        Assert.Equal("Updated description", updated!.Description);
        Assert.Equal(3.0m, updated.Weight);
        Assert.Equal("P-001", updated.PartNumber); // unchanged field preserved
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNullForUnknownId()
    {
        var svc    = new PartService();
        var result = await svc.UpdateAsync(Guid.NewGuid(), new UpdatePartDto { Description = "x" });
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ReplacesDynamicProperties_WhenProvided()
    {
        var svc     = new PartService();
        var created = await svc.CreateAsync(BuildCreateDto());

        var updated = await svc.UpdateAsync(created.Id, new UpdatePartDto
        {
            DynamicProperties = new Dictionary<string, object?> { ["material"] = "steel" }
        });

        Assert.NotNull(updated);
        Assert.False(updated!.DynamicProperties.ContainsKey("color"));
        Assert.Equal("steel", updated.DynamicProperties["material"]?.ToString());
    }

    // ── Delete ────────────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_RemovesPart()
    {
        var svc     = new PartService();
        var created = await svc.CreateAsync(BuildCreateDto());

        var deleted = await svc.DeleteAsync(created.Id);
        Assert.True(deleted);

        var fetched = await svc.GetByIdAsync(created.Id);
        Assert.Null(fetched);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalseForUnknownId()
    {
        var svc    = new PartService();
        var result = await svc.DeleteAsync(Guid.NewGuid());
        Assert.False(result);
    }
}
