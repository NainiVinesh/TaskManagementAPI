using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Data;
using TaskManagementAPI.DTOs;
using System.Security.Claims;
using System.Reflection;
using TaskManagementAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> getAllTasks()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IQueryable<TaskItem> query = _context.Tasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser);

            if (userRole == "Admin")
            {
                query = query;
            }
            else
            {
                // Regular users see:
                // 1. Tasks assigned to them
                // 2. Tasks they created
                query = query.Where(t => t.AssignedtoUserID == userId || t.createdByUserId == userId);
            }

            var tasks = await query
                .Select(t => new TaskResponseDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    Priority = t.Priority,
                    CreatedAt = t.CreatedAt,
                    DueDate = t.DueDate,
                    AssignedToUserID = t.AssignedtoUserID,
                    AssignedToUsername = t.AssignedToUser != null ? t.AssignedToUser.Username : null,

                    CreatedByUserId = t.createdByUserId,
                    CreatedByUsername = t.CreatedByUser.Username
                }).ToListAsync();
            return Ok(tasks);
        }

        //GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTaskById(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var task = await _context.Tasks
                .Include(t => t.AssignedToUser)
                .Include(t => t.CreatedByUser)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound(new {message = "Task not found"});

            //checking authorization
            if (userRole != "Admin" && task.AssignedtoUserID != userId && task.createdByUserId != userId)
                return Forbid(" 404 - Not Allowd to view this task");

            var response = new TaskResponseDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                AssignedToUserID = task.AssignedtoUserID,
                AssignedToUsername = task.AssignedToUser?.Username,
                CreatedByUserId = task.createdByUserId,
                CreatedByUsername = task.CreatedByUser?.Username ?? "Unknown"
            };

            return Ok(response);
        }

        //POST: api/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskCreateDTO taskCreateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            //Validate AssignedToUser exists if provided
            if (taskCreateDto.AssignedToUserId.HasValue)
            {
                var assignedUser = _context.Users.Find(taskCreateDto.AssignedToUserId.Value);
                if (assignedUser == null)
                    return BadRequest("Assigned user does not exist.");
            }
            var task = new TaskItem
            {
                Title = taskCreateDto.Title,
                Description = taskCreateDto.Description,
                Priority = taskCreateDto.Priority,
                CreatedAt = DateTime.UtcNow,
                DueDate = taskCreateDto.DueDate,
                createdByUserId = userId,
                AssignedtoUserID = taskCreateDto.AssignedToUserId
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Load navigation properties for response
            await _context.Entry(task).Reference(t => t.CreatedByUser).LoadAsync();
            if (task.AssignedtoUserID.HasValue)
                await _context.Entry(task).Reference(t => t.AssignedToUser).LoadAsync();

            var response = new TaskResponseDTO
            {
                Id = userId,
                Title = task.Title,
                Description = task.Description,
                Priority = task.Priority,
                CreatedAt = task.CreatedAt,
                DueDate = task.DueDate,
                CreatedByUserId = task.createdByUserId,
                CreatedByUsername = task.CreatedByUser?.Username ?? "Unknown",
                AssignedToUserID = task.AssignedtoUserID,
                AssignedToUsername = task.AssignedToUser?.Username
            };

            return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, response);
        }

        //PUT: api/task/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateDTO taskUpdateDto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var task = await _context.Tasks.FindAsync(id);
            if(task == null)
                return NotFound(new {message = "Task not found"});

            if (userRole != "Admin" && task.createdByUserId != userId)
                return Forbid("You don't have permission to update this task" );

            //Update only provided fields
            if (!string.IsNullOrEmpty(taskUpdateDto.Title))
                task.Title = taskUpdateDto.Title;
            if (!string.IsNullOrEmpty(taskUpdateDto.Description))
                task.Title = taskUpdateDto.Description;
            if (!string.IsNullOrEmpty(taskUpdateDto.Status))
                task.Title = taskUpdateDto.Status;
            if (!string.IsNullOrEmpty(taskUpdateDto.Priority))
                task.Title = taskUpdateDto.Priority;
            if (taskUpdateDto.DueDate.HasValue)
                task.DueDate = taskUpdateDto.DueDate.Value;
            if(taskUpdateDto.AssignedToUserId.HasValue)
            {
                var user = await _context.Users.FindAsync(taskUpdateDto.AssignedToUserId.Value);
                if (user == null)
                    return BadRequest(new { message = "Assigned user doesn't exist" });
                task.AssignedtoUserID = taskUpdateDto.AssignedToUserId;
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "Task Updated Successfully" });    
        }

        //PATCH: api/task/{id}/status
        [HttpPatch("{id}/{status}")]
        public async Task<IActionResult> UpdateTaskStatus(int id, [FromBody] string status)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { message = "Task not found" });
            //Allow update if: Admin, crator or assigned user
            if (userRole != "Admin" && task.createdByUserId != userId && task.AssignedtoUserID != userId)
                return Forbid("You don't have permission to update this task status");

            // Valid status values
            var validStatuses = new[] { "Pending", "In Progress", "Completed" };
            if (!validStatuses.Contains(status))
                return BadRequest(new { message = "Invalid status. Use Pending/InProgress/Completed" });
           
            task.Status = status;
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Task Status updated to {status}" });
        }

        //DELETE: api/task/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete tasks
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
                return NotFound(new { message = "Task not found" });

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent(); //204- Sucess with no response body
        }
        //tested api <---->
    }
}
