using EnterpriseERP.Data;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Services.AI.CEO;

public class CeoDashboardEngine
{
    private readonly ApplicationDbContext _context;

    public CeoDashboardEngine(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CeoDashboardDto> BuildAsync()
    {
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var last30Days = today.AddDays(-30);

        var invoices = await _context.Invoices.ToListAsync();
        var paymentsList = await _context.Payments.ToListAsync();
        var expensesList = await _context.Expenses.ToListAsync();
        var productsList = await _context.Products.ToListAsync();
        var ordersList = await _context.Orders.ToListAsync();
        var quotesList = await _context.Quotes.ToListAsync();

        var revenue = invoices.Sum(i => i.TotalAmount);
        var payments = paymentsList.Sum(p => p.Amount);
        var expenses = expensesList.Sum(e => e.Amount);
        var profit = payments - expenses;
        var monthRevenue = paymentsList.Where(p => p.PaymentDate.Date >= monthStart).Sum(p => p.Amount);
        var monthExpenses = expensesList.Where(e => e.ExpenseDate.Date >= monthStart).Sum(e => e.Amount);
        var avgDailyRevenue = paymentsList.Where(p => p.PaymentDate.Date >= last30Days).Sum(p => p.Amount) / 30m;
        var avgDailyExpenses = expensesList.Where(e => e.ExpenseDate.Date >= last30Days).Sum(e => e.Amount) / 30m;
        var forecastRevenue = avgDailyRevenue * 30m;
        var forecastExpenses = avgDailyExpenses * 30m;
        var marginRate = payments > 0 ? profit / payments * 100m : 0m;
        var collectionRate = revenue > 0 ? payments / revenue * 100m : 100m;
        var cashBurn = avgDailyExpenses - avgDailyRevenue;
        var cashRunwayDays = cashBurn > 0 ? (int)Math.Floor(payments / cashBurn) : 999;

        var unpaidInvoices = invoices
            .Where(i => i.Status == "Unpaid" || i.Status == "Pending")
            .ToList();
        var unpaid = unpaidInvoices.Count;
        var unpaidAmount = unpaidInvoices.Sum(i => i.TotalAmount);
        var lowStock = productsList.Count(p => p.Quantity <= 5);
        var pendingOrders = ordersList.Count(o => o.Status == "Pending");
        var pendingQuotes = quotesList.Count(q => q.Status == "Brouillon" || q.Status == "Pending" || q.Status == "Envoye");

        var presentEmployees = await _context.Presences.CountAsync(p =>
            p.Date.Date == today && p.CheckIn != null && p.CheckOut == null);

        var alerts = new List<string>();
        var recommendations = new List<string>();
        var strategicActions = new List<string>();

        if (unpaid > 0)
        {
            alerts.Add($"AR: {unpaid} facture(s) a encaisser pour {unpaidAmount:N2} EUR.");
            recommendations.Add("Relancer les factures ouvertes avec un message court et une date limite de paiement.");
            strategicActions.Add("Creer une routine quotidienne de recouvrement client.");
        }

        if (lowStock > 0)
        {
            alerts.Add($"Stock: {lowStock} produit(s) sous le seuil critique.");
            recommendations.Add("Preparer un reassort fournisseur pour les articles les plus vendus.");
            strategicActions.Add("Mettre en place des seuils de stock par categorie et marge.");
        }

        if (profit < 0)
        {
            alerts.Add("Finance: resultat negatif detecte.");
            recommendations.Add("Reduire les depenses non essentielles et prioriser les encaissements.");
            strategicActions.Add("Revoir les prix, les remises et les couts d'achat avant les prochaines ventes.");
        }

        if (collectionRate < 70m && revenue > 0)
        {
            alerts.Add($"Cash: taux d'encaissement faible ({collectionRate:N0}%).");
            recommendations.Add("Transformer plus vite les factures emises en paiements recus.");
        }

        if (pendingOrders > 0)
            strategicActions.Add($"Finaliser {pendingOrders} commande(s) en attente pour accelerer le chiffre d'affaires.");

        if (pendingQuotes > 0)
            strategicActions.Add($"Convertir {pendingQuotes} devis ouvert(s) en commande ou facture.");

        if (!alerts.Any())
            alerts.Add("Aucun risque critique detecte.");

        if (!recommendations.Any())
            recommendations.Add("Situation stable. Continuer le suivi quotidien des ventes, paiements, stock et depenses.");

        if (!strategicActions.Any())
            strategicActions.Add("Maintenir la cadence commerciale et surveiller les marges par produit.");

        var score = 100;
        score -= Math.Min(25, unpaid * 5);
        score -= Math.Min(20, lowStock * 4);
        score -= Math.Min(10, pendingOrders * 2);
        if (profit < 0) score -= 20;
        if (marginRate < 10 && payments > 0) score -= 10;
        if (collectionRate < 70 && revenue > 0) score -= 12;
        if (cashRunwayDays < 30) score -= 8;
        score = Math.Clamp(score, 0, 100);

        var riskLevel = score >= 82 ? "Performance" : score >= 65 ? "Surveillance" : "Priorite haute";

        return new CeoDashboardDto
        {
            HealthScore = score,
            Revenue = revenue,
            Payments = payments,
            Expenses = expenses,
            Profit = profit,
            MarginRate = marginRate,
            CollectionRate = collectionRate,
            ForecastRevenue30Days = forecastRevenue,
            ForecastExpenses30Days = forecastExpenses,
            CashRunwayDays = cashRunwayDays,
            Clients = await _context.Clients.CountAsync(),
            Products = productsList.Count,
            Invoices = invoices.Count,
            UnpaidInvoices = unpaid,
            LowStockProducts = lowStock,
            PresentEmployees = presentEmployees,
            Alerts = alerts,
            Recommendations = recommendations,
            StrategicActions = strategicActions,
            PriorityRecommendation = recommendations.FirstOrDefault() ?? "",
            RiskLevel = riskLevel,
            ExecutiveSummary =
                $"Score AI: {score}/100 ({riskLevel}). " +
                $"CA facture: {revenue:N2} EUR. Encaisse: {payments:N2} EUR. " +
                $"Resultat: {profit:N2} EUR, marge nette: {marginRate:N1}%. " +
                $"Mois courant: {monthRevenue:N2} EUR encaisses vs {monthExpenses:N2} EUR de charges. " +
                $"Forecast 30 jours: {forecastRevenue:N2} EUR de revenus et {forecastExpenses:N2} EUR de charges."
        };
    }
}
