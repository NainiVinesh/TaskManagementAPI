namespace TaskManagementAPI.DTOs
{
    public class TaskResponseDTO
    {
        //data:- title, description, status, priority, due date, assigned user
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public DateTime CreatedAt { get; set; } 

        //assignment info
        public int? AssignedToUserID { get; set; }
        public string? AssignedToUsername { get; set; } = string.Empty;

        //creator info
        public int CreatedByUserId { get; set; }
        public string CreatedByUsername { get; set; } = string.Empty;

    }
}
