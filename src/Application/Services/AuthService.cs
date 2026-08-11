using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.Common.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskMasterPro.Domain.Interfaces;
// Para ITokenService
// Para IRefreshTokenRepository
// Borramos las referencias a implementaciones concretas de la infraestructura aquí si no se usan
using Task = System.Threading.Tasks.Task;

namespace TaskManagerPro.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    // CAMBIO: Usamos las INTERFACES, no las clases
    private readonly ITokenService _generateTokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService generateTokenService, // CAMBIO: Interfaz
        IRefreshTokenRepository refreshTokenRepository) // CAMBIO: Interfaz
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _generateTokenService = generateTokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<AuthResponseDto?> Register(RegisterRecordDto registerRecordDto)
    {
        if (!registerRecordDto.Password.Equals(registerRecordDto.Passwordverfication))
            return null;

        try
        {
            var user = new User
            {
                Email = registerRecordDto.Email,
                Password = _passwordHasher.Hash(registerRecordDto.Password)
            };
            await _userRepository.AddAsync(user);

            // Ahora funciona perfectamente a través de la interfaz
            var authResponse = await _generateTokenService.GenerateTokensAsync(user);

            return authResponse;
        }
        catch (Exception)
        {
            throw new Exception("Error en el proceso de registro.");
        }
    }

    public async Task<AuthRecordDto?> Login(LoginRequestDto loginRequestDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);
        if (user == null) return null;

        var isPasswordValid = _passwordHasher.Verify(loginRequestDto.Password, user.Password);

        if (isPasswordValid)
        {
            var authResponse = await _generateTokenService.GenerateTokensAsync(user);
            return new AuthRecordDto(user.Id, user.Email, authResponse.AccessToken);
        }

        return null;
    }

    public async Task Logout(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
        if (storedToken!=null)
        {
            storedToken.IsValid = false;
            await _refreshTokenRepository.UpdateAsync(storedToken);
        }
    }

    public async Task<AuthResponseDto?> RefreshToken(RefreshRequestDto request)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (storedToken == null) throw new Exception("The refresh Token doesn't exist");
        if (storedToken.IsValid == false) throw new Exception("the token has been revoked");
        if (storedToken.ExpiryDate < DateTime.UtcNow) throw new Exception("the refresh token has been expired");
        var user = await _userRepository.GetByIdAsync(storedToken.UserId);
        if (user == null) throw new Exception("user doesn't found");
        storedToken.IsValid = false;
        await _refreshTokenRepository.UpdateAsync(storedToken);
        var authResponse = await _generateTokenService.GenerateTokensAsync(user);
        return authResponse;
    }
}