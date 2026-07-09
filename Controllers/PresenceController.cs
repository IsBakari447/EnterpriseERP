using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class PresenceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PresenceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Présences", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var presences = _context.Presences
                .Include(p => p.Employee)
                .OrderByDescending(p => p.Date)
                .ToList();

            return View(presences);
        }

        [HttpGet]
        [RequirePermission("Présences", "Créer")]
        public IActionResult CheckIn()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            LoadEmployees();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Présences", "Créer")]
        public IActionResult CheckIn(int employeeId)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (employeeId <= 0)
            {
                ViewBag.Error = "Veuillez sélectionner un employé.";
                LoadEmployees();
                return View();
            }

            var employee = _context.Employees.FirstOrDefault(e => e.Id == employeeId);

            if (employee == null)
            {
                ViewBag.Error = "Employé introuvable.";
                LoadEmployees();
                return View();
            }

            var alreadyCheckedIn = _context.Presences.Any(p =>
                p.EmployeeId == employeeId &&
                p.Date == DateTime.Today &&
                p.CheckOut == null);

            if (alreadyCheckedIn)
            {
                ViewBag.Error = "Cet employé a déjà une présence en cours aujourd'hui.";
                LoadEmployees();
                return View();
            }

            var presence = new Presence
            {
                EmployeeId = employeeId,
                Date = DateTime.Today,
                CheckIn = DateTime.Now.TimeOfDay,
                CheckOut = null
            };

            _context.Presences.Add(presence);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Présences",
                "Entrée",
                $"Entrée enregistrée pour {employee.FullName}",
                entityId: presence.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [RequirePermission("Présences", "Modifier")]
        public IActionResult CheckOut()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            LoadOpenPresences();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Présences", "Modifier")]
        public IActionResult CheckOut(int presenceId)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var presence = _context.Presences
                .Include(p => p.Employee)
                .FirstOrDefault(p => p.Id == presenceId);

            if (presence == null)
            {
                ViewBag.Error = "Présence introuvable.";
                LoadOpenPresences();
                return View();
            }

            presence.CheckOut = DateTime.Now.TimeOfDay;
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Présences",
                "Sortie",
                $"Sortie enregistrée pour {presence.Employee?.FullName}",
                entityId: presence.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        private void LoadEmployees()
        {
            ViewBag.Employees = _context.Employees
                .OrderBy(e => e.FullName)
                .ToList();
        }

        private void LoadOpenPresences()
        {
            ViewBag.OpenPresences = _context.Presences
                .Include(p => p.Employee)
                .Where(p => p.Date == DateTime.Today && p.CheckOut == null)
                .ToList();
        }
    }
}