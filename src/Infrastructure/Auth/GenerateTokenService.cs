using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManagerPro.TaskMasterPro.Application.Common.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Domain;
using TaskManagerPro.TaskMasterPro.Domain.Interfaces;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Auth;

public class GenerateTokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly IRefreshTokenRepository _refreshTokenRepo;

    public GenerateTokenService(IConfiguration config, IRefreshTokenRepository refreshTokenRepo)
    {
        _config = config;
        _refreshTokenRepo = refreshTokenRepo;
    }

    // 1. EL MÉTODO PRINCIPAL (FLUJO DE ORQUESTACIÓN)
    public async Task<AuthResponseDto> GenerateTokensAsync(User user)
    {
        // Generamos el Access Token (JWT - 15 min)
        var accessToken = CreateJwtToken(user);

        // Generamos la entidad del Refresh Token de forma segura (1 semana)
        var refreshTokenEntity = CreateRefreshToken(user.Id);

        // Persistimos en MariaDB usando tu repositorio
        await _refreshTokenRepo.SaveAsync(user, refreshTokenEntity);

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenEntity.Token
        };
    }


    // 2. GENERADOR SEGURO DE REFRESH TOKENS (Lógica Criptográfica)
    private RefreshToken CreateRefreshToken(Guid userId)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            UserId = userId,
            CreatedDate = DateTime.UtcNow,
            ExpiryDate = DateTime.UtcNow.AddDays(7), // Tu requerimiento de 1 semana
            IsValid = true
        };
    }

    // 3. GENERADOR DE JWT (Método Privado Encapsulado)
    private string CreateJwtToken(User user)
    {
        var keyString = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(keyString)) return string.Empty;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), 
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}