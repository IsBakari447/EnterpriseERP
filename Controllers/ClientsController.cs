using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Clients", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var clients = _context.Clients
                .OrderByDescending(c => c.CreatedAt)
                .ToList();

            return View(clients);
        }

        [HttpGet]
        [RequirePermission("Clients", "Créer")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Clients", "Créer")]
        public IActionResult Create(Client client)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(client.FullName))
            {
                ViewBag.Error = "Le nom du client est obligatoire.";
                return View(client);
            }

            client.CreatedAt = DateTime.Now;

            _context.Clients.Add(client);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Clients",
                "Création",
                $"Client créé : {client.FullName}",
                entityId: client.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [RequirePermission("Clients", "Modifier")]
        public IActionResult Edit(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var client = _context.Clients.FirstOrDefault(c => c.Id == id);

            if (client == null)
                return NotFound();

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Clients", "Modifier")]
        public IActionResult Edit(Client client)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(client.FullName))
            {
                ViewBag.Error = "Le nom du client est obligatoire.";
                return View(client);
            }

            var existingClient = _context.Clients.FirstOrDefault(c => c.Id == client.Id);

            if (existingClient == null)
                return NotFound();

            string oldValue =
                $"Nom: {existingClient.FullName}, Entreprise: {existingClient.CompanyName}, Email: {existingClient.Email}, Téléphone: {existingClient.Phone}, Adresse: {existingClient.Address}";

            existingClient.FullName = client.FullName;
            existingClient.CompanyName = client.CompanyName;
            existingClient.Email = client.Email;
            existingClient.Phone = client.Phone;
            existingClient.Address = client.Address;

            _context.SaveChanges();

            string newValue =
                $"Nom: {existingClient.FullName}, Entreprise: {existingClient.CompanyName}, Email: {existingClient.Email}, Téléphone: {existingClient.Phone}, Adresse: {existingClient.Address}";

            AuditService.Log(
                _context,
                HttpContext,
                "Clients",
                "Modification",
                $"Client modifié : {existingClient.FullName}",
                oldValue: oldValue,
                newValue: newValue,
                entityId: existingClient.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Clients", "Supprimer")]
        public IActionResult Delete(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var client = _context.Clients.FirstOrDefault(c => c.Id == id);

            if (client == null)
                return NotFound();

            string clientName = client.FullName;
            string entityId = client.Id.ToString();

            _context.Clients.Remove(client);
            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Clients",
                "Suppression",
                $"Client supprimé : {clientName}",
                severity: "Warning",
                entityId: entityId
            );

            return RedirectToAction(nameof(Index));
        }
    }
}