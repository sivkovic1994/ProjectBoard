using TaskService.Models;

namespace TaskService.Services
{
    public interface ITaskService
    {
        Task<TaskItem> CreateTaskAsync(TaskItem task, int userId);
        Task<List<TaskItem>> GetUserTasksAsync(int userId);
        Task<bool> DeleteTaskAsync(int taskId, int userId);
    }
}