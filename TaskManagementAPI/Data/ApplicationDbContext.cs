using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;
namespace TaskManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //DbSets represent tables in ur database
        public DbSet<User> Users { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //configure relationships and constraints
            modelBuilder.Entity<TaskItem>()
                    .HasOne(t => t.AssignedToUser)
                    .WithMany(u => u.AssignedTasks)
                    .HasForeignKey(t => t.AssignedtoUserID)
                    .OnDelete(DeleteBehavior.SetNull); // if user deleted, tasks becomes unassigned

            modelBuilder.Entity<TaskItem>()
                     .HasOne(t => t.CreatedByUser)
                     .WithMany(u => u.CreatedBy)
                     .HasForeignKey(t => t.createdByUserId)
                     .OnDelete(DeleteBehavior.Restrict); // if user deleted, their created tasks are also deleted 

        }
    }
}
