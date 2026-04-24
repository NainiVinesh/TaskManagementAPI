using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
       [Key]
       public int Id { get; set; }
       public string Title { get; set; } = string.Empty;
       public string Description { get; set; } = string.Empty;
       public string Status { get; set; } = string.Empty;
       public string Priority {  get; set; } = string.Empty;
       public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       public DateTime? DueDate { get; set; }
       
       //Foreign Key
       public int? AssignedtoUserID { get; set; }
       public int createdByUserId { get; set; }
       
       [JsonIgnore]
       public User? AssignedToUser { get; set; }
       
       [JsonIgnore]
       public User? CreatedByUser { get; set; }
       
    }
}
