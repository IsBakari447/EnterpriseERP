using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [RequirePermission("Paiements", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var payments = _context.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i!.Client)
                .OrderByDescending(p => p.PaymentDate)
                .ToList();

            return View(payments);
        }

        [HttpGet]
        [RequirePermission("Paiements", "Créer")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            ViewBag.Invoices = _context.Invoices
                .Include(i => i.Client)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Paiements", "Créer")]
        public IActionResult Create(Payment payment)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var invoice = _context.Invoices
                .FirstOrDefault(i => i.Id == payment.InvoiceId);

            if (invoice == null || payment.Amount <= 0)
            {
                ViewBag.Error = "Veuillez sélectionner une facture et saisir un montant valide.";

                ViewBag.Invoices = _context.Invoices
                    .Include(i => i.Client)
                    .OrderByDescending(i => i.InvoiceDate)
                    .ToList();

                return View(payment);
            }

            payment.PaymentDate = DateTime.Now;
            _context.Payments.Add(payment);

            var totalPaid = _context.Payments
                .Where(p => p.InvoiceId == invoice.Id)
                .Sum(p => p.Amount) + payment.Amount;

            invoice.Status = totalPaid >= invoice.TotalAmount ? "Paid" : "Pending";

            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Paiements",
                "Création",
                $"Paiement enregistré : {payment.Amount} € pour la facture {invoice.InvoiceNumber}",
                entityId: payment.Id.ToString()
            );

            return RedirectToAction(nameof(Index));
        }
    }
}