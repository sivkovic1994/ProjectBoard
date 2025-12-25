using System.ComponentModel.DataAnnotations;

namespace TaskService.DTOs
{
    public class CreateTaskDTO
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }
    }
}