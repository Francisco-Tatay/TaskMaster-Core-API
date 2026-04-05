using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration; 
using TaskManagerPro.TaskMasterPro.Domain;

namespace TaskManagerPro.TaskMasterPro.Infrastructure.Services;

public class GenerateToken
{
    private readonly IConfiguration _config;

    // El "Portero" que recibe la configuración del appsettings.json
    public GenerateToken(IConfiguration config)
    {
        _config = config;
    }

    public string generateToken(UserEntity usuario)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        // Ahora sí, sacamos la llave del appsettings.json
        var keyString = _config["Jwt:Key"]; 
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}