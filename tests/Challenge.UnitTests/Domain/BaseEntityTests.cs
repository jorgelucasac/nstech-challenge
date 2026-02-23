using Challenge.Domain.Entities;
using FluentAssertions;
using System.Threading;

namespace Challenge.UnitTests.Domain;

public class BaseEntityTests
{
    private sealed class TestEntity : BaseEntity
    {
    }

    [Fact]
    public void UpdateTimestamps_ShouldUpdateUpdatedAt()
    {
        var entity = new TestEntity();
        var originalUpdatedAt = entity.UpdatedAt;

        Thread.Sleep(5);
        entity.UpdateTimestamps();

        entity.UpdatedAt.Should().BeAfter(originalUpdatedAt);
    }

    [Fact]
    public void SoftDelete_ShouldMarkInactiveAndUpdateDeletedAt()
    {
        var entity = new TestEntity();
        var originalDeletedAt = entity.DeletedAt;

        Thread.Sleep(5);
        entity.SoftDelete();

        entity.IsActive.Should().BeFalse();
        entity.DeletedAt.Should().BeAfter(originalDeletedAt);
    }
}
