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
        var taskItemDtos = result.ToList();
        taskItemDtos.Count().ShouldBe(1);
        var dto = taskItemDtos.First();
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
        //arrange
        var repo = Substitute.For<ITaskRepository>();
        var idgen = Substitute.For<IIdGenerator>();
        var expectedId = Guid.NewGuid();
        idgen.NewId().Returns(expectedId);
        var sut = new TaskServices(repo, idgen);
        repo.GetByIdAsync(expectedId).Returns(new TaskEntity { Id = expectedId, Description = "test", Title = "hola" });
        //act 
        await sut.UpdateTaskAsync(new TaskItemDto(expectedId, "test", "description"));
        //assert
        await repo.Received(1)
            .UpdateAsync(Arg.Is<TaskEntity>(t => t.Title == "test" && t.Description == "description"));
    }

    [Fact]
    public async Task UpdateTaskAsync_TaskNotFound_DoesNotCallUpdate()
    {
        //arrange : el repo dice que no existe ninguna tarea explicitamente
        var repo = Substitute.For<ITaskRepository>();
        repo.GetByIdAsync(Arg.Any<Guid>()).Returns((TaskEntity?)null);
        var sut = new TaskServices(repo, Substitute.For<IIdGenerator>());
        //act intenta actualizr la tarea inesistente 
        await sut.UpdateTaskAsync(new TaskItemDto(Guid.NewGuid(), "x", null));
        await repo.DidNotReceive().UpdateAsync(Arg.Any<TaskEntity>());
    }
}