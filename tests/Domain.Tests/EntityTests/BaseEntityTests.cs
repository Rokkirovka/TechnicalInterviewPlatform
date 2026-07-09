using Domain.Entities;

namespace Domain.Tests.EntityTests;

public class BaseEntityTests
{
    private class TestEntity : BaseEntity
    {
    }

    [Fact]
    public void CreatedAt_DefaultValue_IsUtcNow()
    {
        var before = DateTime.UtcNow;
        var entity = new TestEntity();
        var after = DateTime.UtcNow;

        Assert.InRange(entity.CreatedAt, before, after);
    }

    [Fact]
    public void UpdatedAt_DefaultValue_IsNull()
    {
        var entity = new TestEntity();

        Assert.Null(entity.UpdatedAt);
    }

    [Fact]
    public void DeletedAt_DefaultValue_IsNull()
    {
        var entity = new TestEntity();

        Assert.Null(entity.DeletedAt);
    }

    [Fact]
    public void Id_DefaultValue_IsZero()
    {
        var entity = new TestEntity();

        Assert.Equal(0, entity.Id);
    }

    [Fact]
    public void CanSetAndGetProperties()
    {
        var entity = new TestEntity
        {
            Id = 42,
            UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            DeletedAt = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        Assert.Equal(42, entity.Id);
        Assert.Equal(new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), entity.UpdatedAt);
        Assert.Equal(new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc), entity.DeletedAt);
    }
}
