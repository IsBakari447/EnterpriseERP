using EnterpriseERP.Data;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Services.AI;

public class EnterpriseAiEngine
{
    private readonly ApplicationDbContext _context;

    public EnterpriseAiEngine(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> AskAsync(string question)
    {
        question = question.ToLowerInvariant();

        if (question.Contains("stock") || question.Contains("rupture"))
            return await StockAnalysisAsync();

        if (question.Contains("facture") || question.Contains("impay") || question.Contains("relance"))
            return await InvoiceAnalysisAsync();

        if (question.Contains("present") || question.Contains("presence") || question.Contains("employ"))
            return await PresenceAnalysisAsync();

        if (question.Contains("cash") || question.Contains("tresorerie") || question.Contains("forecast") || question.Contains("prevision"))
            return await ForecastAnalysisAsync();

        if (question.Contains("marge") || question.Contains("profit") || question.Contains("vente") || question.Contains("finance") || question.Contains("resume"))
            return await FinanceAnalysisAsync();

        if (question.Contains("client"))
            return await ClientAnalysisAsync();

        if (question.Contains("priorite") || question.Contains("risque") || question.Contains("strategie") || question.Contains("action"))
            return await PriorityAnalysisAsync();

        return await GlobalSummaryAsync();
    }

    public async Task<object> GetDashboardAsync()
    {
        var today = DateTime.Today;
        var last30Days = today.AddDays(-30);

        var clients = await _context.Clients.CountAsync();
        var products = await _context.Products.CountAsync();
        var invoices = await _context.Invoices.ToListAsync();
        var paymentsList = await _context.Payments.ToListAsync();
        var expensesList = await _context.Expenses.ToListAsync();

        var payments = paymentsList.Sum(p => p.Amount);
        var expenses = expensesList.Sum(e => e.Amount);
        var profit = payments - expenses;
        var revenue = invoices.Sum(i => i.TotalAmount);
        var collectionRate = revenue > 0 ? payments / revenue * 100m : 100m;
        var marginRate = payments > 0 ? profit / payments * 100m : 0m;
        var forecastRevenue30Days = paymentsList.Where(p => p.PaymentDate.Date >= last30Days).Sum(p => p.Amount);
        var forecastExpenses30Days = expensesList.Where(e => e.ExpenseDate.Date >= last30Days).Sum(e => e.Amount);
        var lowStockCount = await _context.Products.CountAsync(p => p.Quantity <= 5);
        var unpaidInvoicesCount = invoices.Count(i => i.Status == "Unpaid" || i.Status == "Pending");

        var todayPresences = await _context.Presences
            .CountAsync(p => p.Date.Date == today && p.CheckIn != null && p.CheckOut == null);

        var alerts = new List<string>();

        if (lowStockCount > 0)
            alerts.Add($"Stock critique: {lowStockCount} produit(s) sous le seuil.");

        if (unpaidInvoicesCount > 0)
            alerts.Add($"Recouvrement: {unpaidInvoicesCount} facture(s) a relancer.");

        if (profit < 0)
            alerts.Add("Rentabilite: resultat negatif detecte.");

        if (collectionRate < 70m && revenue > 0)
            alerts.Add($"Cash: taux d'encaissement faible ({collectionRate:N0}%).");

        if (!alerts.Any())
            alerts.Add("Aucun risque critique detecte pour le moment.");

        var recommendation =
            lowStockCount > 0
                ? "Priorite: securiser les produits en stock faible avant les prochaines ventes."
                : unpaidInvoicesCount > 0
                    ? "Priorite: relancer les clients avec factures impayees et confirmer une date de paiement."
                    : profit < 0
                        ? "Priorite: revoir les depenses et les prix de vente."
                        : "Situation stable. Continuer le suivi ventes, paiements, stock et depenses.";

        return new
        {
            clients,
            products,
            invoices = invoices.Count,
            payments,
            expenses,
            profit,
            marginRate,
            collectionRate,
            forecastRevenue30Days,
            forecastExpenses30Days,
            lowStockCount,
            unpaidInvoicesCount,
            todayPresences,
            alerts,
            recommendation
        };
    }

    private async Task<string> FinanceAnalysisAsync()
    {
        var invoicesTotal = await SumDecimalAsync(_context.Invoices.Select(i => i.TotalAmount));
        var paymentsTotal = await SumDecimalAsync(_context.Payments.Select(p => p.Amount));
        var expensesTotal = await SumDecimalAsync(_context.Expenses.Select(e => e.Amount));
        var profit = paymentsTotal - expensesTotal;
        var marginRate = paymentsTotal > 0 ? profit / paymentsTotal * 100m : 0m;
        var collectionRate = invoicesTotal > 0 ? paymentsTotal / invoicesTotal * 100m : 100m;

        return
            "Finance AI\n\n" +
            $"- Facture: {invoicesTotal:N2} EUR\n" +
            $"- Encaisse: {paymentsTotal:N2} EUR\n" +
            $"- Depenses: {expensesTotal:N2} EUR\n" +
            $"- Resultat: {profit:N2} EUR\n" +
            $"- Marge nette: {marginRate:N1}%\n" +
            $"- Taux d'encaissement: {collectionRate:N1}%\n\n" +
            BuildFinancialAdvice(profit, marginRate, collectionRate);
    }

    private async Task<string> ForecastAnalysisAsync()
    {
        var today = DateTime.Today;
        var last30Days = today.AddDays(-30);
        var payments30 = await SumDecimalAsync(_context.Payments
            .Where(p => p.PaymentDate.Date >= last30Days)
            .Select(p => p.Amount));
        var expenses30 = await SumDecimalAsync(_context.Expenses
            .Where(e => e.ExpenseDate.Date >= last30Days)
            .Select(e => e.Amount));
        var projectedProfit = payments30 - expenses30;
        var dailyBurn = (expenses30 - payments30) / 30m;
        var totalCash = await SumDecimalAsync(_context.Payments.Select(p => p.Amount));
        var runway = dailyBurn > 0 ? Math.Floor(totalCash / dailyBurn) : 999m;

        return
            "Forecast AI 30 jours\n\n" +
            $"- Revenus projetes: {payments30:N2} EUR\n" +
            $"- Charges projetees: {expenses30:N2} EUR\n" +
            $"- Resultat projete: {projectedProfit:N2} EUR\n" +
            $"- Autonomie cash indicative: {(runway >= 999 ? "stable" : runway.ToString("N0") + " jour(s)")}\n\n" +
            "Conseil AI: suivre chaque semaine l'ecart entre encaissements reels et charges engagees.";
    }

    private async Task<string> StockAnalysisAsync()
    {
        var lowStock = await _context.Products
            .Where(p => p.Quantity <= 5)
            .OrderBy(p => p.Quantity)
            .Select(p => new { p.Name, p.Quantity, p.PurchasePrice, p.SalePrice })
            .Take(10)
            .ToListAsync();

        if (!lowStock.Any())
            return "Stock AI\n\nAucun produit en stock faible. Continue a surveiller les articles a forte rotation.";

        return "Stock AI - Produits critiques\n\n" +
               string.Join("\n", lowStock.Select(p => $"- {p.Name}: {p.Quantity} restant(s), marge unitaire estimee {(p.SalePrice - p.PurchasePrice):N2} EUR")) +
               "\n\nConseil AI: reapprovisionner d'abord les produits a forte marge et forte demande.";
    }

    private async Task<string> InvoiceAnalysisAsync()
    {
        var unpaid = await _context.Invoices
            .Include(i => i.Client)
            .Where(i => i.Status == "Unpaid" || i.Status == "Pending")
            .OrderByDescending(i => i.TotalAmount)
            .Take(10)
            .Select(i => new
            {
                i.InvoiceNumber,
                Client = i.Client != null ? i.Client.FullName : "",
                i.TotalAmount,
                i.Status
            })
            .ToListAsync();

        if (!unpaid.Any())
            return "Recouvrement AI\n\nAucune facture impayee ou en attente.";

        return "Recouvrement AI - Factures prioritaires\n\n" +
               string.Join("\n", unpaid.Select(i => $"- {i.InvoiceNumber} | {i.Client} | {i.TotalAmount:N2} EUR | {i.Status}")) +
               "\n\nConseil AI: commencer par les plus gros montants, puis envoyer une relance courte avec date de paiement attendue.";
    }

    private async Task<string> PresenceAnalysisAsync()
    {
        var today = DateTime.Today;

        var presences = await _context.Presences
            .Include(p => p.Employee)
            .Where(p => p.Date.Date == today)
            .Select(p => new
            {
                Employee = p.Employee != null ? p.Employee.FullName : "",
                Status = p.CheckIn == null ? "Absent" : p.CheckOut == null ? "Present" : "Sorti"
            })
            .ToListAsync();

        if (!presences.Any())
            return "RH AI\n\nAucune presence enregistree aujourd'hui.";

        return "RH AI - Presences aujourd'hui\n\n" +
               string.Join("\n", presences.Select(p => $"- {p.Employee}: {p.Status}"));
    }

    private async Task<string> ClientAnalysisAsync()
    {
        var clientsCount = await _context.Clients.CountAsync();
        var latest = await _context.Clients
            .OrderByDescending(c => c.CreatedAt)
            .Take(5)
            .Select(c => c.FullName)
            .ToListAsync();

        return
            "Clients AI\n\n" +
            $"- Total clients: {clientsCount}\n" +
            $"- Derniers clients:\n{string.Join("\n", latest.Select(c => "- " + c))}\n\n" +
            "Conseil AI: identifier les clients rentables, les clients inactifs et les factures ouvertes avant chaque campagne.";
    }

    private async Task<string> PriorityAnalysisAsync()
    {
        var dashboard = await GetDashboardAsync();
        var json = System.Text.Json.JsonSerializer.Serialize(dashboard);
        return
            "Priorites AI\n\n" +
            "1. Encaisser les factures ouvertes et confirmer les dates de paiement.\n" +
            "2. Securiser le stock faible avant rupture.\n" +
            "3. Comparer marge, depenses et prix de vente avant remise.\n" +
            "4. Convertir les devis et commandes ouverts.\n\n" +
            $"Signal dashboard: {json}";
    }

    private async Task<string> GlobalSummaryAsync()
    {
        var clients = await _context.Clients.CountAsync();
        var products = await _context.Products.CountAsync();
        var invoices = await _context.Invoices.CountAsync();
        var payments = await SumDecimalAsync(_context.Payments.Select(p => p.Amount));
        var expenses = await SumDecimalAsync(_context.Expenses.Select(e => e.Amount));

        return
            "Resume EnterpriseERP AI\n\n" +
            $"- Clients: {clients}\n" +
            $"- Produits: {products}\n" +
            $"- Factures: {invoices}\n" +
            $"- Encaissements: {payments:N2} EUR\n" +
            $"- Depenses: {expenses:N2} EUR\n" +
            $"- Resultat: {(payments - expenses):N2} EUR\n\n" +
            "Questions utiles: stock faible, factures impayees, forecast cash, marge, priorites, clients, presences.";
    }

    private static string BuildFinancialAdvice(decimal profit, decimal marginRate, decimal collectionRate)
    {
        if (profit < 0)
            return "Conseil AI: le resultat est negatif. Reduis les charges non essentielles et relance les paiements en retard.";

        if (collectionRate < 70m)
            return "Conseil AI: la priorite est l'encaissement. Les ventes existent, mais trop de valeur reste dans les factures ouvertes.";

        if (marginRate < 12m)
            return "Conseil AI: la marge est fragile. Revois prix, remises, couts fournisseurs et transport.";

        return "Conseil AI: situation saine. Augmente le volume sur les clients et produits les plus rentables.";
    }

    private static async Task<decimal> SumDecimalAsync(IQueryable<decimal> query)
    {
        var values = await query.ToListAsync();
        return values.Sum();
    }
}
