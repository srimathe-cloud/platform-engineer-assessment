namespace PlatformEngineer.Worker.Models
{
    public class WorkItem
    {
        public int Id { get; set; }

        public string ApplicationName { get; set; } = string.Empty;

        public string Environment { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }
    }
}
