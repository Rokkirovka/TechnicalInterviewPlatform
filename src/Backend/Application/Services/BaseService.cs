using Application.Dtos;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Services;

public abstract class BaseService<TEntity, TDto, TCreateRequest, TUpdateRequest>(
    IRepository<TEntity> repository,
    IDeletionLogRepository<TEntity> deletionLogRepository,
    IMapper mapper)
    : IBaseService<TDto, TCreateRequest, TUpdateRequest>
    where TEntity : BaseEntity
    where TDto : BaseDto
{
    protected readonly IRepository<TEntity> Repository = repository;
    protected readonly IMapper Mapper = mapper;

    public virtual async Task<IReadOnlyList<TDto>> GetAllAliveAsync()
    {
        var entities = await Repository.AllAliveAsync();
        return Mapper.Map<IReadOnlyList<TDto>>(entities);
    }

    public virtual async Task<IReadOnlyList<TDto>> GetAllDeletedAsync()
    {
        var entities = await Repository.AllDeletedAsync();
        return Mapper.Map<IReadOnlyList<TDto>>(entities);
    }

    public virtual async Task<IReadOnlyList<TDto>> GetAllAsync()
    {
        var entities = await Repository.AllAsync();
        return Mapper.Map<IReadOnlyList<TDto>>(entities);
    }

    public virtual async Task<TDto?> GetByIdAsync(int id)
    {
        var entity = await Repository.GetByIdAsync(id);
        return entity == null ? null : Mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto?> GetByIdIncludingDeletedAsync(int id)
    {
        var entity = await Repository.GetByIdIncludingDeletedAsync(id);
        return entity == null ? null : Mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> CreateAsync(TCreateRequest request)
    {
        var entity = Mapper.Map<TEntity>(request);
        var result = await Repository.AddAsync(entity);
        return Mapper.Map<TDto>(result);
    }

    public virtual async Task<TDto> UpdateAsync(TUpdateRequest request)
    {
        var entity = await Repository.GetByIdAsync(Mapper.Map<TEntity>(request).Id);
        if (entity == null)
            throw new Exception($"Сущность с id не найдена");

        Mapper.Map(request, entity);
        await Repository.UpdateAsync(entity);
        return Mapper.Map<TDto>(entity);
    }

    public virtual async Task DeleteAsync(int id, int deletedByUserId, string? reason = null)
    {
        var entity = await Repository.GetByIdAsync(id);
        if (entity == null)
            throw new Exception($"Сущность с id {id} не найдена");

        if (entity.DeletedAt != null)
            throw new Exception($"Сущность с id {id} уже удалена");

        entity.DeletedAt = DateTime.UtcNow;
        await Repository.UpdateAsync(entity);

        await deletionLogRepository.AddAsync(entity, deletedByUserId, reason);
    }

    public virtual async Task RestoreAsync(int id)
    {
        var entity = await Repository.GetByIdIncludingDeletedAsync(id);
        if (entity == null)
            throw new Exception($"Сущность с id {id} не найдена");

        if (entity.DeletedAt == null)
            throw new Exception($"Сущность с id {id} не удалена");

        entity.DeletedAt = null;
        await Repository.UpdateAsync(entity);
    }
}