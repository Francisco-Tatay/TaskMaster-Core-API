using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskManagerPro.TaskMasterPro.Domain;

public class TaskEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public Boolean IsCompleted { get; set; }

    public int UserId { get; set; }

    // 2. Esta es la clave lógica (el objeto). 
    // Le decimos: "Oye EF, tu jefe para esta relación es UserId"
    [ForeignKey("UserId")]
    [JsonIgnore] // Para que no te vuelva a dar el error 400 de antes
    public UserEntity? User { get; set; }
}