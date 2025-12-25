using Microsoft.EntityFrameworkCore;
using TaskService.Data;
using TaskService.Models;

namespace TaskService.Services
{
    public class TaskService : ITaskService
    {
        private readonly TaskDbContext _dbContext;

        public TaskService(TaskDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItem task, int userId)
        {
            task.UserId = userId;
            
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return task;
        }

        public async Task<List<TaskItem>> GetUserTasksAsync(int userId)
        {
            return await _dbContext.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            TaskItem? task = await _dbContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
                return false;

            _dbContext.Tasks.Remove(task);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}