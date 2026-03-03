using PartsApi.DTOs;

namespace PartsApi.Services;

public interface IPartService
{
    Task<IEnumerable<PartDto>> GetAllAsync();
    Task<PartDto?> GetByIdAsync(Guid id);
    Task<PartDto> CreateAsync(CreatePartDto dto);
    Task<PartDto?> UpdateAsync(Guid id, UpdatePartDto dto);
    Task<bool> DeleteAsync(Guid id);
}
