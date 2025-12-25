using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskService.DTOs;
using TaskService.Models;
using TaskService.Services;

namespace TaskService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        #region Public Methods

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                TaskResponseDTO result = await _taskService.CreateTaskAsync(dto, GetUserId());
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating task");
                return StatusCode(500, new { message = "Failed to create task" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                List<TaskResponseDTO> tasks = await _taskService.GetUserTasksAsync(GetUserId());
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching tasks");
                return StatusCode(500, new { message = "Failed to fetch tasks" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                TaskResponseDTO? result = await _taskService.UpdateTaskAsync(id, dto, GetUserId());

                if (result == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task");
                return StatusCode(500, new { message = "Failed to update task" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                bool deleted = await _taskService.DeleteTaskAsync(id, GetUserId());

                if (!deleted)
                    return NotFound(new { message = "Task not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting task");
                return StatusCode(500, new { message = "Failed to delete task" });
            }
        }

        #endregion

        #region Helpers

        private int GetUserId()
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedAccessException("User ID claim is missing or invalid");

            return userId;
        }

        #endregion
    }
}