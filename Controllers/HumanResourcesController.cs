using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers;

public class HumanResourcesController : Controller
{
    private readonly ApplicationDbContext _context;

    public HumanResourcesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [RequirePermission("RH", "Voir")]
    public IActionResult Index()
    {
        ViewBag.Employees = _context.Employees.OrderBy(e => e.FullName).ToList();
        ViewBag.LeaveRequests = _context.LeaveRequests.Include(l => l.Employee).OrderByDescending(l => l.CreatedAt).Take(20).ToList();
        ViewBag.Schedules = _context.HrSchedules.Include(s => s.Employee).OrderBy(s => s.WorkDate).Take(20).ToList();
        ViewBag.PayrollSlips = _context.PayrollSlips.Include(p => p.Employee).OrderByDescending(p => p.Period).Take(20).ToList();
        ViewBag.Documents = _context.HrDocuments.Include(d => d.Employee).OrderByDescending(d => d.CreatedAt).Take(20).ToList();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("RH", "Créer")]
    public IActionResult CreateLeave(LeaveRequest request)
    {
        if (request.EndDate < request.StartDate)
            request.EndDate = request.StartDate;

        _context.LeaveRequests.Add(request);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("RH", "Modifier")]
    public IActionResult UpdateLeaveStatus(int id, string status)
    {
        var request = _context.LeaveRequests.Find(id);
        if (request == null)
            return NotFound();

        request.Status = string.IsNullOrWhiteSpace(status) ? "En attente" : status;
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("RH", "Créer")]
    public IActionResult CreateSchedule(HrSchedule schedule)
    {
        _context.HrSchedules.Add(schedule);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("RH", "Créer")]
    public IActionResult CreatePayroll(PayrollSlip slip)
    {
        _context.PayrollSlips.Add(slip);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("RH", "Créer")]
    public IActionResult CreateDocument(HrDocument document)
    {
        _context.HrDocuments.Add(document);
        _context.SaveChanges();
        return RedirectToAction(nameof(Index));
    }
}
