using System.ComponentModel.DataAnnotations;
namespace TaskManagementAPI.DTOs
{
    public class TaskUpdateDTO
    {
        //date :- title, description, status, priority, due date, assigned user
       [MaxLength(200)]
       public string? Title { get; set; }
       [MaxLength(1000)]
       public string? Description { get; set; }
       public string? Status { get; set; }
       public string? Priority { get; set; }
       public DateTime? DueDate { get; set; }
       public int? AssignedToUserId { get; set; }
    }
}
