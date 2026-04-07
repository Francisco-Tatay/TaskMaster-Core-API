using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskMasterPro.Infrastructure.Services;

namespace TaskManagerPro.TaskMasterPro.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly GenerateTokenService  _generateTokenService;

    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher, GenerateTokenService generateTokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _generateTokenService = generateTokenService;
    }
    /*
    public async Task<UserEntity?> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;
        var isValid=_passwordHasher.Verify(password, user.Password);
        return isValid ? user : null;

    }
    */
    
    public async Task Register(string email, string password)
    {
        try
        {
            var user = new UserEntity();
            user.Email = email;
            user.Password = _passwordHasher.Hash(password);
            await _userRepository.AddAsync(user);
        }
        catch (Exception exception)
        {
            throw new Exception(exception.Message + " Error inserting a new user ");
        }
    }
    public Task Logout()
    {
        throw new NotImplementedException();
    }
    
    public async Task<AuthRecordDto> Login(LoginRequestDto loginRequestDto)
    {

        var user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);
        if (user == null) return null;
        
        var isPasswordValid = _passwordHasher.Verify(loginRequestDto.Password, user.Password);
        
        if (isPasswordValid)
        {
            return new AuthRecordDto(user.Id, user.Email,_generateTokenService.GenerateToken(user));
        }
        
        return null;
    }
}