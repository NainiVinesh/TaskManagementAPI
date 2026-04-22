using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Models
{
    public class TaskItem
    {
       [Key]
       public string Id { get; set; }
       public string Title { get; set; } = string.Empty;
       public string Description { get; set; } = string.Empty;
       public string status { get; set; } = string.Empty;
       public string priority {  get; set; } = string.Empty;
       public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
       public DateTime DueDate { get; set; }
       
       //Foreign Key
       public int? AssignedtoUserID { get; set; }
       public int createdByUserId { get; set; }
       
       [JsonIgnore]
       public User? AssignedToUser { get; set; }
       
       [JsonIgnore]
       public User? CreatedByUser { get; set; }
       
    }
}
