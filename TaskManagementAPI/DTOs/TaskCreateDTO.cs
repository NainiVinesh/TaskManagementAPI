using System.ComponentModel.DataAnnotations;
namespace TaskManagementAPI.DTOs
{
    public class TaskCreateDTO
    {
        //data:- title, description, priority, due date, assigned to user id 
        [Required(ErrorMessage = "Title is required")]
        [MinLength(3)]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty; // 0: Low, 1: Medium, 2: High
        public DateTime? DueDate { get; set; }

        public int? AssignedToUserId { get; set; }
    }
}