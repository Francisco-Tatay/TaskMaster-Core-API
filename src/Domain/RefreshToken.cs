using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TaskManagerPro.TaskMasterPro.Domain;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public bool IsValid { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedDate { get; set; }
    [ForeignKey("UserId")]
    [JsonIgnore] // Para que no te vuelva a dar el error 400 de antes
    public User? User { get; set; }
}