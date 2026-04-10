using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskMasterPro.Infrastructure.Auth;
using TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;
using TaskManagerPro.TaskMasterPro.Infrastructure.Services;
using Task = System.Threading.Tasks.Task;

namespace TaskManagerPro.TaskMasterPro.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly GenerateTokenService _generateTokenService;
    private readonly RefreshTokenRepository _refreshTokenRepository;
    public AuthService(IUserRepository userRepository, IPasswordHasher passwordHasher,
        GenerateTokenService generateTokenService, RefreshTokenRepository refreshTokenRepository)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _generateTokenService = generateTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }
    /*
    public async Task<User?> Login(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null) return null;
        var isValid=_passwordHasher.Verify(password, user.Password);
        return isValid ? user : null;

    }
    */

    public async Task<AuthResponseDto?> Register(RegisterRecordDto registerRecordDto)
    {
        // 1. Validar contraseñas
        if (!registerRecordDto.Password.Equals(registerRecordDto.Passwordverfication))
            return null;

        try {
            // 2. Crear y guardar al usuario
            var user = new User {
                Email = registerRecordDto.Email,
                Password = _passwordHasher.Hash(registerRecordDto.Password)
            };
            await _userRepository.AddAsync(user);

            // 3. ¡AQUÍ ESTÁ LA LLAVE! Generamos los tokens de una vez
            var accessToken = _generateTokenService.GenerateTokensAsync(user); // La de 15 min
          //  var refreshToken = _generateTokenService.CreateRefreshToken(); // La de larga duración

            // 4. Guardamos el Refresh Token en la DB (tu nueva tabla)
            await _refreshTokenRepository.SaveAsync(user.Id, accessToken);

            return new AuthResponseDto {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        catch (Exception) {
            throw new Exception("Error en el proceso de registro.");
        }
    }
    public Task Logout()
    {
        throw new NotImplementedException();
    }

    public async Task<AuthRecordDto?> Login(LoginRequestDto loginRequestDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);
#pragma warning disable CS8603 // Possible null reference return.
        if (user == null) return null;
#pragma warning restore CS8603 // Possible null reference return.

        var isPasswordValid = _passwordHasher.Verify(loginRequestDto.Password, user.Password);

        if (isPasswordValid)
        {
            var authResponse = await _generateTokenService.GenerateTokensAsync(user);
            return new AuthRecordDto(user.Id, user.Email, authResponse.AccessToken);
        }

        return null;
    }
}