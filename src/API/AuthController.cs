using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using TaskManagerPro.Application.Services;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;

namespace TaskManagerPro.API;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
    public async Task<IActionResult> Login(LoginRequestDto? loginRequestDto)
    {
        if (loginRequestDto == null) return BadRequest("Login data is required.");
        if (string.IsNullOrWhiteSpace(loginRequestDto.Email) || string.IsNullOrWhiteSpace(loginRequestDto.Password))
            return BadRequest("Email or password is required.");
        try
        {
            var isValid = await _authService.Login(loginRequestDto);
            if (isValid == null) return Unauthorized();
            return Ok(isValid);
        }
        catch (Exception e)
        {
            Console.WriteLine("error " + e.Message);
        }

        return null!;
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequestDto request)
    {
        await _authService.Logout(request.RefreshToken);
        return Ok(new { message = "Logout succesful" });
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRecordDto? registerRecordDto)
    {
        if (registerRecordDto == null) return BadRequest("register data is required.");
        if (string.IsNullOrWhiteSpace(registerRecordDto.Email) ||
            string.IsNullOrWhiteSpace(registerRecordDto.Password) ||
            string.IsNullOrWhiteSpace(registerRecordDto.Passwordverfication))
            return BadRequest("Information invalid try again");
        try
        {
            var isValid = await _authService.Register(registerRecordDto);
            if (isValid==null) return Conflict("Try again if the problem persists contact support");
            return Ok(isValid);

        }
        catch (Exception e)
        {
            Console.WriteLine("error " + e.Message);
        }


#pragma warning disable CS8603 // Possible null reference return.
        return null;
#pragma warning restore CS8603 // Possible null reference return.
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshRequestDto request)
    {
        try
        {
            var response = await _authService.RefreshToken(request);
            if (response == null) return Unauthorized(new { message = "Token invalid" });
            return Ok(response);
        }
        catch (Exception ex)
        {
            return Unauthorized(new {message = ex.Message});
        }
    }
}