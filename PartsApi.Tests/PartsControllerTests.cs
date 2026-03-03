using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using PartsApi.DTOs;
using PartsApi.Models;

namespace PartsApi.Tests;

public class PartsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        Converters = { new JsonStringEnumConverter() },
        PropertyNameCaseInsensitive = true
    };

    public PartsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    private static async Task<T?> ReadJson<T>(HttpResponseMessage response)
        => await response.Content.ReadFromJsonAsync<T>(JsonOpts);

    private static CreatePartDto BuildDto(string partNumber = "TEST-001") => new()
    {
        PartNumber  = partNumber,
        Description = "Integration test part",
        Uom         = UomType.Kilogram,
        Weight      = 2.5m,
        Demarcation = "Zone-B",
        MakeFlags = new List<FlagItemDto>
        {
            new() { Name = "IsMake", Value = true, ValidFrom = DateTime.UtcNow }
        },
        DynamicProperties = new Dictionary<string, object?> { ["finish"] = "matte" }
    };

    [Fact]
    public async Task POST_CreatePart_Returns201WithBody()
    {
        var response = await _client.PostAsJsonAsync("/api/parts", BuildDto());

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var created = await ReadJson<PartDto>(response);
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created!.Id);
        Assert.Equal("TEST-001", created.PartNumber);
    }

    [Fact]
    public async Task GET_AllParts_ReturnsOk()
    {
        await _client.PostAsJsonAsync("/api/parts", BuildDto("GET-ALL-001"));

        var response = await _client.GetAsync("/api/parts");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var parts = await ReadJson<List<PartDto>>(response);
        Assert.NotNull(parts);
        Assert.NotEmpty(parts!);
    }

    [Fact]
    public async Task GET_PartById_ReturnsCorrectPart()
    {
        var created = (await ReadJson<PartDto>(await _client.PostAsJsonAsync("/api/parts", BuildDto("GET-ID-001"))))!;

        var response = await _client.GetAsync($"/api/parts/{created.Id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var fetched = await ReadJson<PartDto>(response);
        Assert.Equal(created.Id, fetched!.Id);
    }

    [Fact]
    public async Task GET_PartById_Returns404_WhenNotFound()
    {
        var response = await _client.GetAsync($"/api/parts/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PUT_UpdatePart_ReturnsUpdatedPart()
    {
        var created = (await ReadJson<PartDto>(await _client.PostAsJsonAsync("/api/parts", BuildDto("PUT-001"))))!;

        var update = new UpdatePartDto { Description = "Updated via PUT", Weight = 9.9m };
        var response = await _client.PutAsJsonAsync($"/api/parts/{created.Id}", update);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await ReadJson<PartDto>(response);
        Assert.Equal("Updated via PUT", updated!.Description);
        Assert.Equal(9.9m, updated.Weight);
        Assert.Equal("PUT-001", updated.PartNumber);
    }

    [Fact]
    public async Task DELETE_Part_Returns204()
    {
        var created = (await ReadJson<PartDto>(await _client.PostAsJsonAsync("/api/parts", BuildDto("DEL-001"))))!;

        var response = await _client.DeleteAsync($"/api/parts/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var getResponse = await _client.GetAsync($"/api/parts/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task POST_CreatePart_ReturnsBadRequest_WhenBodyInvalid()
    {
        var invalid = new { };
        var response = await _client.PostAsJsonAsync("/api/parts", invalid);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
