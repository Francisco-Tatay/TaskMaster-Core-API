using FluentAssertions;
using NSubstitute;
using TaskManagerPro.Application.DTOs.Tasks;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.Services;
using TaskEntity = TaskManagerPro.TaskMasterPro.Domain.Task;

namespace TaskManagerPro.test.TestTaskManager;

public class TaskServiceTest
{
    [Fact]
    public async Task GetUserTasksAsync_WithTasks_ReturnsMappedDtos()
    {
        // ARRANGE
        var repo = Substitute.For<ITaskRepository>();
        var idGen = Substitute.For<IIdGenerator>();
        var sut = new TaskServices(repo, idGen);

        var fakeTask = new TaskEntity
        {
            Id = Guid.NewGuid(),
            Title = "Buy bread",
            Description = "At the bakery",
            UserId = Guid.NewGuid()
        };

        repo.GetAllByUserIdAsync(Arg.Any<Guid>())
            .Returns(new[] { fakeTask });

        // ACT
        var result = await sut.GetUserTasksAsync(Guid.NewGuid());

        // ASSERT
        result.Should().HaveCount(1);
        var dto = result.First();
        dto.Id.Should().Be(fakeTask.Id);
        dto.Title.Should().Be("Buy bread");
        dto.Description.Should().Be("At the bakery");
    }

    [Fact]
    public async Task GetUserTasksAsync_NoTask_ReturnsEmptyList()
    {
        var repo = Substitute.For<ITaskRepository>();
        var idGen = Substitute.For<IIdGenerator>();
        repo.GetAllByUserIdAsync(Arg.Any<Guid>()).Returns(Array.Empty<TaskEntity>());
        var sut = new TaskServices(repo, idGen);
        var result = await sut.GetUserTasksAsync(Guid.NewGuid());
        result.Should().BeEmpty();
    }
}