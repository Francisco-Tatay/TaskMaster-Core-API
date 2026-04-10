namespace TaskManagerPro.TaskMasterPro.Application.DTOs.Auth;

public record RegisterRecordDto(
    string Email,
    string Password,
    string Passwordverfication
);