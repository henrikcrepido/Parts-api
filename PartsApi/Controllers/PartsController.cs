using Microsoft.AspNetCore.Mvc;
using PartsApi.DTOs;
using PartsApi.Services;

namespace PartsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    /// <summary>Returns all parts.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PartDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var parts = await _partService.GetAllAsync();
        return Ok(parts);
    }

    /// <summary>Returns a single part by its ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var part = await _partService.GetByIdAsync(id);
        return part is null ? NotFound() : Ok(part);
    }

    /// <summary>Creates a new part.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PartDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePartDto dto)
    {
        var created = await _partService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates an existing part (patch semantics – only supplied fields are changed).</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePartDto dto)
    {
        var updated = await _partService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Deletes a part by its ID.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _partService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
