using Shouldly;
using NSubstitute;
using TaskManagerPro.Application.DTOs.Tasks;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.Services;
using TaskEntity = TaskManagerPro.TaskMasterPro.Domain.Task;

namespace TestTaskManager;

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
        result.Count().ShouldBe(1);
        var dto = result.First();
        dto.Id.ShouldBe(fakeTask.Id);
        dto.Title.ShouldBe("Buy bread");
        dto.Description.ShouldBe("At the bakery");
    }

    [Fact]
    public async Task GetUserTasksAsync_NoTask_ReturnsEmptyList()
    {
        var repo = Substitute.For<ITaskRepository>();
        var idGen = Substitute.For<IIdGenerator>();
        repo.GetAllByUserIdAsync(Arg.Any<Guid>()).Returns(Array.Empty<TaskEntity>());
        var sut = new TaskServices(repo, idGen);
        var result = await sut.GetUserTasksAsync(Guid.NewGuid());
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateTaskAsync_ValidateData_AddsEntityWithNewId()
    {
        //arrange
        var repo = Substitute.For<ITaskRepository>();
        var idGen = Substitute.For<IIdGenerator>();
        var expectedId = Guid.NewGuid();
        idGen.NewId().Returns(expectedId);
        var sut = new TaskServices(repo, idGen);
        var userId = Guid.NewGuid();
        //act 
        await sut.CreateTaskAsync(new TaskItemDto(Guid.Empty, "test", "description"), userId);
        //assert
        await repo.Received(1).AddAsync(Arg.Is<TaskEntity>(t =>
            t.Id == expectedId && t.UserId == userId && t.Title == "test" && !t.IsCompleted));
    }

    [Fact]
    public async Task UpdateTaskAync_ValidateData_UpdatesEntity()
    {
        var repo = Substitute.For<ITaskRepository>();
        var idgen = Substitute.For<IIdGenerator>();
        var expectedId = Guid.NewGuid();
        idgen.NewId().Returns(expectedId);
        var sut = new TaskServices(repo, idgen);
    }
}