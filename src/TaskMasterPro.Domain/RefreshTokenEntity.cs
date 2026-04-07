using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskManagerPro.TaskMasterPro.Domain;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    public int UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public bool IsValid { get; set; }
    [ForeignKey("UserId")]
    [JsonIgnore] // Para que no te vuelva a dar el error 400 de antes
    public UserEntity? User { get; set; }
}