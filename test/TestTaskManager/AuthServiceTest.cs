using Shouldly;
using NSubstitute;
using
    TaskManagerPro.TaskMasterPro.Application.DTOs.Auth; // RegisterRecordDto, LoginRequestDto, AuthResponseDto, AuthRecordDto
using TaskManagerPro.TaskMasterPro.Domain; // User, RefreshToken (aquí no hay conflicto con "Task")
using TaskManagerPro.TaskMasterPro.Domain.Interfaces; // IUserRepository, IRefreshTokenRepository
using TaskManagerPro.TaskMasterPro.Application.Common.Interfaces; // ITokenService
using TaskManagerPro.TaskManagerPro.Interfaces; // IPasswordHasher
using TaskManagerPro.Application.Services;

using Task = System.Threading.Tasks.Task; // AuthService

namespace TestTaskManager;

public class AuthServiceTest
{
    [Fact]
    public async Task Register_PasswordsDoNotMatch_Return_Null()
    {
        //constructors
        var userRepo = Substitute.For<IUserRepository>();
        var haser = Substitute.For<IPasswordHasher>();
        var tokenService = Substitute.For<ITokenService>();
        var refreshRepo = Substitute.For<IRefreshTokenRepository>();
        var sut = new AuthService(userRepo, haser, tokenService, refreshRepo);
        var register = await sut.Register(new RegisterRecordDto("test@test.com", "1111", "2222"));
        await userRepo.DidNotReceive().AddAsync(Arg.Any<User>());
        register.ShouldBeNull();
    }

    [Fact]
    public async Task Register_ValidData_HashesAndAddsUser()
    {
        //arrange
        var userRepo = Substitute.For<IUserRepository>();
        var hasher = Substitute.For<IPasswordHasher>();
        var tokenService = Substitute.For<ITokenService>();
        var refreshRepo = Substitute.For<IRefreshTokenRepository>();
        var sut = new AuthService(userRepo, hasher, tokenService, refreshRepo);
        hasher.Hash("1111").Returns("hashed-password"); // programo el hash
        var expected = new AuthResponseDto { AccessToken = "access-token", RefreshToken = "refresh-token" };
        tokenService.GenerateTokensAsync(Arg.Any<User>()).Returns(expected); // objeto completo
        //act 
        var result = await sut.Register(new RegisterRecordDto("test@test.com", "1111", "1111"));

        hasher.Received(1).Hash("1111");
        await userRepo.Received(1)
            .AddAsync(Arg.Is<User>(u => u.Email == "test@test.com" && u.Password == "hashed-password"));

        result.ShouldBeSameAs(expected);
    }

    [Fact]
    public async Task Login_UserNotFound_ReturnsNull()
    {
        //arrange ( 4 pattern)
        var userRepo = Substitute.For<IUserRepository>();
        var hasher = Substitute.For<IPasswordHasher>();
        var tokenService = Substitute.For<ITokenService>();
        var refreshRepo = Substitute.For<IRefreshTokenRepository>();
        var sut = new AuthService(userRepo, hasher, tokenService, refreshRepo);
        userRepo.GetByEmailAsync("test@test.com").Returns((User?)null);

        var result = await sut.Login(new LoginRequestDto("test@test.com", "1111"));
        result.ShouldBeNull();
        await userRepo.Received(1).GetByEmailAsync("test@test.com");
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsNull()
    {
        var userRepo = Substitute.For<IUserRepository>();
        var hasher = Substitute.For<IPasswordHasher>();
        var tokenService = Substitute.For<ITokenService>();
        var refreshRepo = Substitute.For<IRefreshTokenRepository>();
        var sut = new AuthService(userRepo, hasher, tokenService, refreshRepo);
        userRepo.GetByEmailAsync("test@test.com").Returns((User?)null);
        var existingUser = new User { Id = Guid.NewGuid(), Email = "test@test.com", Password = "hashed-password" };

        userRepo.GetByEmailAsync("test@test.com").Returns(existingUser);
        hasher.Verify("1111", "hashed-password").Returns(false);
        var result = await sut.Login(new LoginRequestDto("test@test.com", "1111"));
        result.ShouldBeNull();
        hasher.Received(1).Verify("1111","hashed-password");
        
    }
}