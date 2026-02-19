namespace AspireApp1.DbApi.Models
{
    public class ProjectTask
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime EndAtUtc { get; set; } = DateTime.UtcNow.AddDays(1);
        public ProjectTaskStatus Status { get; set; } = ProjectTaskStatus.NotStarted;
        public string? PerformedBy { get; set; } // Username who performs this task
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public Project Project { get; set; } = null!;
    }
}
