using Microsoft.AspNetCore.Mvc;
using TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;
using TaskManagerPro.TaskMasterPro.Application.Services;

namespace TaskManagerPro.TaskMasterPro.API;

[ApiController]
[Route("/Auth/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
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


        return null;
    }
}