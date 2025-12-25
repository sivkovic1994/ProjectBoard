using Microsoft.EntityFrameworkCore;
using TaskService.Data;
using TaskService.DTOs;
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

        public async Task<TaskResponseDTO> CreateTaskAsync(CreateTaskDTO dto, int userId)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title cannot be empty");

            var task = new TaskItem
            {
                Title = dto.Title.Trim(),
                Description = dto.Description?.Trim(),
                UserId = userId
            };
            
            _dbContext.Tasks.Add(task);
            await _dbContext.SaveChangesAsync();

            return new TaskResponseDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt
            };
        }

        public async Task<List<TaskResponseDTO>> GetUserTasksAsync(int userId)
        {
            return await _dbContext.Tasks
                .Where(t => t.UserId == userId)
                .Select(t => new TaskResponseDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    IsCompleted = t.IsCompleted,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<TaskResponseDTO?> UpdateTaskAsync(int taskId, UpdateTaskDTO dto, int userId)
        {
            TaskItem? task = await _dbContext.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            if (task == null)
                return null;

            if (dto.Title != null)
            {
                if (string.IsNullOrWhiteSpace(dto.Title))
                    throw new ArgumentException("Title cannot be empty");
                task.Title = dto.Title.Trim();
            }

            if (dto.Description != null)
                task.Description = dto.Description.Trim();

            if (dto.IsCompleted.HasValue)
                task.IsCompleted = dto.IsCompleted.Value;

            await _dbContext.SaveChangesAsync();

            return new TaskResponseDTO
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt
            };
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