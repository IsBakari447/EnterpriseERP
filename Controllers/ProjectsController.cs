using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers;

public class ProjectsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProjectsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [RequirePermission("Projets", "Voir")]
    public IActionResult Index()
    {
        ViewBag.Projects = _context.ProjectBoards.Include(p => p.Tasks).OrderByDescending(p => p.CreatedAt).ToList();
        ViewBag.Tasks = _context.ProjectTaskItems.Include(t => t.ProjectBoard).OrderBy(t => t.Deadline).ToList();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Projets", "Créer")]
    public IActionResult CreateProject(ProjectBoard project)
    {
        project.Progress = Math.Clamp(project.Progress, 0, 100);
        _context.ProjectBoards.Add(project);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Projets", "Créer")]
    public IActionResult CreateTask(ProjectTaskItem task)
    {
        task.Progress = Math.Clamp(task.Progress, 0, 100);
        _context.ProjectTaskItems.Add(task);
        _context.SaveChanges();
        UpdateProjectProgress(task.ProjectBoardId);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Projets", "Modifier")]
    public IActionResult MoveTask(int id, string status)
    {
        var task = _context.ProjectTaskItems.Find(id);
        if (task == null)
            return NotFound();

        task.Status = string.IsNullOrWhiteSpace(status) ? "A faire" : status;
        task.Progress = task.Status == "Termine" ? 100 : task.Progress;
        _context.SaveChanges();
        UpdateProjectProgress(task.ProjectBoardId);
        return RedirectToAction(nameof(Index));
    }

    private void UpdateProjectProgress(int projectId)
    {
        var project = _context.ProjectBoards.Include(p => p.Tasks).FirstOrDefault(p => p.Id == projectId);
        if (project == null || project.Tasks.Count == 0)
            return;

        project.Progress = (int)Math.Round(project.Tasks.Average(t => t.Progress));
        _context.SaveChanges();
    }
}
