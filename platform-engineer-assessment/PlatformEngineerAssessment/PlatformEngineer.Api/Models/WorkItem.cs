namespace PlatformEngineer.Api.Models
{
    public class WorkItem
    {
        public int Id { get; set; }

        public string ApplicationName { get; set; } = string.Empty;

        public string Environment { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // When the item was moved to processed list
        public DateTime? ProcessedAt { get; set; }
    }
}