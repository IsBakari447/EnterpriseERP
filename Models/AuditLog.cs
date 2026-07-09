using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        public string UserName { get; set; } = "";

        public string UserEmail { get; set; } = "";

        public string UserRole { get; set; } = "";

        public string Module { get; set; } = "";

        public string Action { get; set; } = "";

        public string Description { get; set; } = "";

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public string? IpAddress { get; set; }

        public string? Browser { get; set; }

        public string? OperatingSystem { get; set; }

        public string? MachineName { get; set; }

        public bool Success { get; set; } = true;

        public string Severity { get; set; } = "Information";

        public string? EntityId { get; set; }

        public string? SessionId { get; set; }
    }
}