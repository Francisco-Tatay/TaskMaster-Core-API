namespace TaskManagerPro.Application.Common.Model;

public record PageResponse<T>(
    List<T> Items,
    int PageNumber,
    int TotalPages
    );