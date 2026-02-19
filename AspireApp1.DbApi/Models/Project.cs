namespace AspireApp1.DbApi.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int CustomerId { get; set; }
        public int? CustomerOrderId { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Wip;
        public string? ProjectReader { get; set; } // Username who can read/view this project
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Customer Customer { get; set; } = null!;
        public CustomerOrder? CustomerOrder { get; set; }
    }
}
