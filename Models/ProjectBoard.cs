using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class ProjectBoard
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";

    public string ClientName { get; set; } = "";

    public DateTime Deadline { get; set; } = DateTime.Today.AddDays(30);

    public int Progress { get; set; }

    public string Status { get; set; } = "Actif";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public List<ProjectTaskItem> Tasks { get; set; } = new();
}
