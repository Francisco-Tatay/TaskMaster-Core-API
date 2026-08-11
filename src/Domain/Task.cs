using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskManagerPro.TaskMasterPro.Domain;

public class Task
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Boolean IsCompleted { get; set; }


    // 2. Esta es la clave lógica (el objeto). 
    [ForeignKey("UserId")]
    [JsonIgnore] // Para que no te vuelva a dar el error 400 de antes
    public User? User { get; set; }
}