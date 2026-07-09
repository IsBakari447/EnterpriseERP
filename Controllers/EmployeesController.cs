using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Employés", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var employees = _context.Employees
                .OrderByDescending(e => e.CreatedAt)
                .ToList();

            return View(employees);
        }

        [HttpGet]
        [RequirePermission("Employés", "Créer")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Employés", "Créer")]
        public IActionResult Create(Employee employee)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(employee.FullName))
            {
                ViewBag.Error = "Le nom complet est obligatoire.";
                return View(employee);
            }

            employee.CreatedAt = DateTime.Now;

            _context.Employees.Add(employee);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Employés",
                "Ajout",
                $"Nouvel employé : {employee.FullName}",
                entityId: employee.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [RequirePermission("Employés", "Modifier")]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Employés", "Modifier")]
        public IActionResult Edit(Employee employee)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(employee.FullName))
            {
                ViewBag.Error = "Le nom complet est obligatoire.";
                return View(employee);
            }

            var existingEmployee = _context.Employees.FirstOrDefault(e => e.Id == employee.Id);

            if (existingEmployee == null)
                return NotFound();

            string oldValue =
                $"Nom: {existingEmployee.FullName}, Poste: {existingEmployee.Position}, Email: {existingEmployee.Email}, Téléphone: {existingEmployee.Phone}, Salaire: {existingEmployee.Salary}";

            existingEmployee.FullName = employee.FullName;
            existingEmployee.Position = employee.Position;
            existingEmployee.Email = employee.Email;
            existingEmployee.Phone = employee.Phone;
            existingEmployee.Salary = employee.Salary;

            _context.SaveChanges();

            string newValue =
                $"Nom: {existingEmployee.FullName}, Poste: {existingEmployee.Position}, Email: {existingEmployee.Email}, Téléphone: {existingEmployee.Phone}, Salaire: {existingEmployee.Salary}";

            AuditService.Log(
                _context,
                HttpContext,
                "Employés",
                "Modification",
                $"Employé modifié : {existingEmployee.FullName}",
                oldValue: oldValue,
                newValue: newValue,
                entityId: existingEmployee.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [RequirePermission("Employés", "Supprimer")]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
                return NotFound();

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Employés", "Supprimer")]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var employee = _context.Employees.FirstOrDefault(e => e.Id == id);

            if (employee == null)
                return NotFound();

            string employeeName = employee.FullName;
            string entityId = employee.Id.ToString();

            _context.Employees.Remove(employee);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Employés",
                "Suppression",
                $"Employé supprimé : {employeeName}",
                severity: "Warning",
                entityId: entityId
            );

            return RedirectToAction(nameof(Index));
        }
    }
}