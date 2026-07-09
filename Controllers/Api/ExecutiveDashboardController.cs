using Microsoft.AspNetCore.Authorization;
using EnterpriseERP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers.Api
{
    [ApiController]
    [Route("api/executive-dashboard")]
[Authorize]
    public class ExecutiveDashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ExecutiveDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var yearStart = new DateTime(today.Year, 1, 1);

            var invoices = await _context.Invoices.ToListAsync();
            var payments = await _context.Payments.ToListAsync();
            var products = await _context.Products.ToListAsync();
            var orders = await _context.Orders.ToListAsync();
            var quotes = await _context.Quotes.ToListAsync();
            var expenses = await _context.Expenses.ToListAsync();

            var totalRevenue = payments.Sum(p => p.Amount);
            var monthRevenue = payments
                .Where(p => p.PaymentDate >= monthStart)
                .Sum(p => p.Amount);

            var yearRevenue = payments
                .Where(p => p.PaymentDate >= yearStart)
                .Sum(p => p.Amount);

            var totalExpenses = expenses.Sum(e => e.Amount);
            var profit = totalRevenue - totalExpenses;

            var unpaidInvoices = invoices
                .Where(i => i.Status == "Pending" || i.Status == "Unpaid")
                .ToList();

            var pendingOrders = orders
                .Where(o => o.Status == "Pending")
                .ToList();

            var pendingQuotes = quotes
                .Where(q => q.Status == "Brouillon" || q.Status == "Pending" || q.Status == "Envoyé")
                .ToList();

            var lowStockProducts = products
                .Where(p => p.Quantity <= 5)
                .ToList();

            return Ok(new
            {
                totalRevenue,
                monthRevenue,
                yearRevenue,
                totalExpenses,
                profit,

                totalClients = await _context.Clients.CountAsync(),
                totalProducts = products.Count,
                totalOrders = orders.Count,
                totalQuotes = quotes.Count,
                totalInvoices = invoices.Count,

                unpaidInvoicesCount = unpaidInvoices.Count,
                unpaidInvoicesAmount = unpaidInvoices.Sum(i => i.TotalAmount),

                pendingOrdersCount = pendingOrders.Count,
                pendingQuotesCount = pendingQuotes.Count,
                lowStockCount = lowStockProducts.Count,

                alerts = new[]
                {
                    new { type = "Factures", message = $"{unpaidInvoices.Count} facture(s) impayée(s)" },
                    new { type = "Stock", message = $"{lowStockProducts.Count} produit(s) en stock faible" },
                    new { type = "Commandes", message = $"{pendingOrders.Count} commande(s) en attente" }
                }
            });
        }
    }
}
