using System.ComponentModel.DataAnnotations;

namespace TaskService.DTOs
{
    public class UpdateTaskDTO
    {
        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public bool? IsCompleted { get; set; }
    }
}