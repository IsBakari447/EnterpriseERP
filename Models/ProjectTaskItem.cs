using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models;

public class ProjectTaskItem
{
    public int Id { get; set; }

    [Required]
    public int ProjectBoardId { get; set; }

    public ProjectBoard? ProjectBoard { get; set; }

    [Required]
    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public string AssignedTo { get; set; } = "";

    public DateTime Deadline { get; set; } = DateTime.Today.AddDays(7);

    public string Status { get; set; } = "A faire";

    public int Progress { get; set; }
}
