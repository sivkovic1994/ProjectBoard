using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        #region Public Methods

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
        {
            TaskItem result = await _taskService.CreateTaskAsync(task, GetUserId());
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTasks()
        {
            List<TaskItem> tasks = await _taskService.GetUserTasksAsync(GetUserId());
            return Ok(tasks);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            bool deleted = await _taskService.DeleteTaskAsync(id, GetUserId());

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        #endregion

        #region Helpers

        private int GetUserId()
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return 0;

            return int.Parse(userIdClaim);
        }

        #endregion
    }
}
