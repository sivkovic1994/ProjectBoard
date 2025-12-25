using TaskService.DTOs;

namespace TaskService.Services
{
    public interface ITaskService
    {
        Task<TaskResponseDTO> CreateTaskAsync(CreateTaskDTO dto, int userId);
        Task<List<TaskResponseDTO>> GetUserTasksAsync(int userId);
        Task<TaskResponseDTO?> UpdateTaskAsync(int taskId, UpdateTaskDTO dto, int userId);
        Task<bool> DeleteTaskAsync(int taskId, int userId);
    }
}