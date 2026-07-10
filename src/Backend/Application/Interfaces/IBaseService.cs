using Application.Dtos;

namespace Application.Interfaces;

public interface IBaseService<TDto, in TCreateRequest, in TUpdateRequest> where TDto : BaseDto
{
    Task<IReadOnlyList<TDto>> GetAllAliveAsync();
    Task<IReadOnlyList<TDto>> GetAllDeletedAsync();
    Task<IReadOnlyList<TDto>> GetAllAsync();
    Task<TDto?> GetByIdAsync(int id);
    Task<TDto?> GetByIdIncludingDeletedAsync(int id);
    Task<TDto> CreateAsync(TCreateRequest request);
    Task<TDto> UpdateAsync(TUpdateRequest request);
    Task DeleteAsync(int id, int deletedByUserId, string? reason = null);
    Task RestoreAsync(int id);
}