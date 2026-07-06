namespace Domain.Entities;

public class DeletionLogBase<TEntity> where TEntity : BaseEntity
{
    public int Id { get; set; }
    public int DeletedByUserId { get; set; }
    public int EntityId { get; set; }
    public string? DeletionReason { get; set; }
    public DateTime DeletedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User DeletedByUser { get; set; } = null!;
    public virtual TEntity Entity { get; set; } = null!;
}