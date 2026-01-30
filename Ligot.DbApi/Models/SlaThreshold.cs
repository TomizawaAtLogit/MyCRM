namespace Ligot.DbApi.Models;

public class SlaThreshold
{
    public int Id { get; set; }
    public CasePriority Priority { get; set; }
    public int ResponseTimeHours { get; set; } // First response time in hours
    public int ResolutionTimeHours { get; set; } // Resolution time in hours
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

