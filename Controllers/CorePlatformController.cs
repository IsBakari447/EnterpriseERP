using System.Globalization;
using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseERP.Controllers;

public class CorePlatformController : Controller
{
    private readonly ApplicationDbContext _context;

    public CorePlatformController(ApplicationDbContext context)
    {
        _context = context;
    }

    [RequirePermission("Paramètres", "Voir")]
    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("UserEmail") == null)
            return RedirectToAction("Login", "Account");

        ViewBag.AutomationRules = await _context.AutomationRules
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        ViewBag.ExternalIntegrations = await _context.ExternalIntegrations
            .OrderBy(x => x.Provider)
            .ThenBy(x => x.Name)
            .ToListAsync();
        ViewBag.ImportJobs = await _context.DataImportJobs
            .OrderByDescending(x => x.CreatedAt)
            .Take(10)
            .ToListAsync();
        ViewBag.DynamicReports = await _context.DynamicReports
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
        ViewBag.CollaborationComments = await _context.CollaborationComments
            .OrderByDescending(x => x.CreatedAt)
            .Take(20)
            .ToListAsync();
        ViewBag.DataVersions = await _context.DataVersions
            .OrderByDescending(x => x.CreatedAt)
            .Take(20)
            .ToListAsync();
        ViewBag.TenantAccounts = await _context.TenantAccounts
            .OrderBy(x => x.Name)
            .ToListAsync();
        ViewBag.MarketplaceExtensions = await _context.MarketplaceExtensions
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .ToListAsync();
        ViewBag.CustomFields = await _context.CustomFieldDefinitions
            .OrderBy(x => x.EntityType)
            .ThenBy(x => x.FieldKey)
            .ToListAsync();

        ViewBag.FeatureStatus = BuildFeatureStatus();
        ViewBag.AdvancedFeatureStatus = BuildAdvancedFeatureStatus();

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> SaveAutomation(AutomationRule rule)
    {
        if (string.IsNullOrWhiteSpace(rule.Name))
            return RedirectToAction(nameof(Index));

        rule.ConditionsJson = string.IsNullOrWhiteSpace(rule.ConditionsJson) ? "{}" : rule.ConditionsJson;
        rule.ActionPayloadJson = string.IsNullOrWhiteSpace(rule.ActionPayloadJson) ? "{}" : rule.ActionPayloadJson;
        rule.CreatedAt = DateTime.UtcNow;

        _context.AutomationRules.Add(rule);
        await _context.SaveChangesAsync();

        AuditService.Log(_context, HttpContext, "Automations", "Creation", $"Workflow cree : {rule.Name}");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> DeleteAutomation(int id)
    {
        var rule = await _context.AutomationRules.FindAsync(id);
        if (rule != null)
        {
            _context.AutomationRules.Remove(rule);
            await _context.SaveChangesAsync();
            AuditService.Log(_context, HttpContext, "Automations", "Suppression", $"Workflow supprime : {rule.Name}");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> SaveIntegration(ExternalIntegration integration)
    {
        if (string.IsNullOrWhiteSpace(integration.Name))
            return RedirectToAction(nameof(Index));

        integration.SettingsJson = string.IsNullOrWhiteSpace(integration.SettingsJson) ? "{}" : integration.SettingsJson;
        integration.CreatedAt = DateTime.UtcNow;

        _context.ExternalIntegrations.Add(integration);
        await _context.SaveChangesAsync();

        AuditService.Log(_context, HttpContext, "Integrations", "Creation", $"Integration creee : {integration.Provider} - {integration.Name}");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> DeleteIntegration(int id)
    {
        var integration = await _context.ExternalIntegrations.FindAsync(id);
        if (integration != null)
        {
            _context.ExternalIntegrations.Remove(integration);
            await _context.SaveChangesAsync();
            AuditService.Log(_context, HttpContext, "Integrations", "Suppression", $"Integration supprimee : {integration.Provider} - {integration.Name}");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> ImportCsv(string module, IFormFile? file)
    {
        var job = new DataImportJob
        {
            Module = module ?? "",
            FileName = file?.FileName ?? "",
            CreatedBy = HttpContext.Session.GetString("UserEmail"),
            Status = "Failed"
        };

        try
        {
            if (file == null || file.Length == 0)
                throw new InvalidOperationException("Aucun fichier CSV fourni.");

            using var reader = new StreamReader(file.OpenReadStream());
            var rows = new List<string[]>();
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                    rows.Add(ParseCsvLine(line));
            }

            if (rows.Count <= 1)
                throw new InvalidOperationException("Le fichier CSV ne contient aucune ligne de donnees.");

            var imported = module switch
            {
                "Clients" => ImportClients(rows.Skip(1)),
                "Products" => ImportProducts(rows.Skip(1)),
                _ => throw new InvalidOperationException("Module d'import non supporte.")
            };

            job.RowsImported = imported;
            job.Status = "Completed";
            await _context.SaveChangesAsync();

            AuditService.Log(_context, HttpContext, "Import", "CSV", $"Import {module} : {imported} ligne(s)");
        }
        catch (Exception ex)
        {
            job.ErrorSummary = ex.Message;
            job.RowsFailed = 1;
        }

        _context.DataImportJobs.Add(job);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> SaveReport(DynamicReport report)
    {
        if (string.IsNullOrWhiteSpace(report.Name))
            return RedirectToAction(nameof(Index));

        report.MetricsJson = string.IsNullOrWhiteSpace(report.MetricsJson) ? "{}" : report.MetricsJson;
        report.FiltersJson = string.IsNullOrWhiteSpace(report.FiltersJson) ? "{}" : report.FiltersJson;
        report.CreatedAt = DateTime.UtcNow;
        report.CreatedBy = HttpContext.Session.GetString("UserEmail");

        _context.DynamicReports.Add(report);
        await _context.SaveChangesAsync();
        AuditService.Log(_context, HttpContext, "Reporting", "Creation", $"Rapport dynamique cree : {report.Name}");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> SaveComment(CollaborationComment comment)
    {
        if (string.IsNullOrWhiteSpace(comment.EntityType) || string.IsNullOrWhiteSpace(comment.Body))
            return RedirectToAction(nameof(Index));

        comment.EntityId = string.IsNullOrWhiteSpace(comment.EntityId) ? "global" : comment.EntityId;
        comment.CreatedAt = DateTime.UtcNow;
        comment.CreatedBy = HttpContext.Session.GetString("UserEmail");

        _context.CollaborationComments.Add(comment);
        await _context.SaveChangesAsync();
        AuditService.Log(_context, HttpContext, "Collaboration", "Commentaire", $"Commentaire ajoute sur {comment.EntityType}/{comment.EntityId}");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> SaveVersion(DataVersion version)
    {
        if (string.IsNullOrWhiteSpace(version.EntityType) || string.IsNullOrWhiteSpace(version.EntityId))
            return RedirectToAction(nameof(Index));

        version.SnapshotJson = string.IsNullOrWhiteSpace(version.SnapshotJson) ? "{}" : version.SnapshotJson;
        version.VersionNumber = Math.Max(1, version.VersionNumber);
        version.CreatedAt = DateTime.UtcNow;
        version.CreatedBy = HttpContext.Session.GetString("UserEmail");

        _context.DataVersions.Add(version);
        await _context.SaveChangesAsync();
        AuditService.Log(_context, HttpContext, "Versioning", "Snapshot", $"Version {version.VersionNumber} creee pour {version.EntityType}/{version.EntityId}");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> SaveTenant(TenantAccount tenant)
    {
        if (string.IsNullOrWhiteSpace(tenant.Name))
            return RedirectToAction(nameof(Index));

        tenant.Slug = string.IsNullOrWhiteSpace(tenant.Slug)
            ? Slugify(tenant.Name)
            : Slugify(tenant.Slug);
        tenant.CreatedAt = DateTime.UtcNow;

        _context.TenantAccounts.Add(tenant);
        await _context.SaveChangesAsync();
        AuditService.Log(_context, HttpContext, "MultiTenant", "Creation", $"Tenant cree : {tenant.Name}");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> SaveExtension(MarketplaceExtension extension)
    {
        if (string.IsNullOrWhiteSpace(extension.Name))
            return RedirectToAction(nameof(Index));

        extension.CreatedAt = DateTime.UtcNow;

        _context.MarketplaceExtensions.Add(extension);
        await _context.SaveChangesAsync();
        AuditService.Log(_context, HttpContext, "Marketplace", "Extension", $"Extension ajoutee : {extension.Name}");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> SaveCustomField(CustomFieldDefinition field)
    {
        if (string.IsNullOrWhiteSpace(field.EntityType) || string.IsNullOrWhiteSpace(field.FieldKey))
            return RedirectToAction(nameof(Index));

        field.FieldKey = Slugify(field.FieldKey).Replace('-', '_');
        field.OptionsJson = string.IsNullOrWhiteSpace(field.OptionsJson) ? "[]" : field.OptionsJson;
        field.CreatedAt = DateTime.UtcNow;

        _context.CustomFieldDefinitions.Add(field);
        await _context.SaveChangesAsync();
        AuditService.Log(_context, HttpContext, "CustomFields", "Creation", $"Champ custom cree : {field.EntityType}.{field.FieldKey}");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Paramètres", "Modifier")]
    public async Task<IActionResult> DeleteEntity(string type, int id)
    {
        object? entity = type switch
        {
            "report" => await _context.DynamicReports.FindAsync(id),
            "comment" => await _context.CollaborationComments.FindAsync(id),
            "version" => await _context.DataVersions.FindAsync(id),
            "tenant" => await _context.TenantAccounts.FindAsync(id),
            "extension" => await _context.MarketplaceExtensions.FindAsync(id),
            "field" => await _context.CustomFieldDefinitions.FindAsync(id),
            _ => null
        };

        if (entity != null)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            AuditService.Log(_context, HttpContext, "CorePlatform", "Suppression", $"Suppression {type} #{id}");
        }

        return RedirectToAction(nameof(Index));
    }

    private int ImportClients(IEnumerable<string[]> rows)
    {
        var count = 0;
        foreach (var row in rows)
        {
            if (row.Length == 0 || string.IsNullOrWhiteSpace(row[0]))
                continue;

            _context.Clients.Add(new Client
            {
                FullName = row.ElementAtOrDefault(0)?.Trim() ?? "",
                CompanyName = row.ElementAtOrDefault(1)?.Trim() ?? "",
                Email = row.ElementAtOrDefault(2)?.Trim() ?? "",
                Phone = row.ElementAtOrDefault(3)?.Trim() ?? "",
                Address = row.ElementAtOrDefault(4)?.Trim() ?? ""
            });
            count++;
        }

        return count;
    }

    private int ImportProducts(IEnumerable<string[]> rows)
    {
        var count = 0;
        foreach (var row in rows)
        {
            if (row.Length == 0 || string.IsNullOrWhiteSpace(row[0]))
                continue;

            _context.Products.Add(new Product
            {
                Name = row.ElementAtOrDefault(0)?.Trim() ?? "",
                Category = row.ElementAtOrDefault(1)?.Trim() ?? "",
                PurchasePrice = ParseDecimal(row.ElementAtOrDefault(2)),
                SalePrice = ParseDecimal(row.ElementAtOrDefault(3)),
                Quantity = ParseInt(row.ElementAtOrDefault(4))
            });
            count++;
        }

        return count;
    }

    private static decimal ParseDecimal(string? value)
    {
        return decimal.TryParse(value?.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }

    private static int ParseInt(string? value)
    {
        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }

    private static string[] ParseCsvLine(string line)
    {
        return line.Split(';').Length > 1
            ? line.Split(';')
            : line.Split(',');
    }

    private static string Slugify(string value)
    {
        var chars = value.Trim().ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray();

        return string.Join("-", new string(chars).Split('-', StringSplitOptions.RemoveEmptyEntries));
    }

    private static List<(string Feature, string Status, string Details)> BuildFeatureStatus() => new()
    {
        ("Authentification email", "Actif", "Login email + mot de passe hash, JWT API et sessions web."),
        ("SSO / OAuth", "Pret a configurer", "Connecteurs references pour Google, Microsoft et OAuth2 via integrations."),
        ("MFA", "Configure", "Option EnableTwoFactorAuth presente dans les parametres securite."),
        ("Utilisateurs & roles", "Actif", "Users, roles, permissions granulaires et Admin Center."),
        ("Dashboard personnalise", "Actif", "DashboardLayout, KPI, AI et CEO Dashboard."),
        ("CRUD donnees metier", "Actif", "Clients, fournisseurs, produits, devis, factures, commandes, paiements, presences, depenses."),
        ("Recherche avancee", "Actif", "Recherche globale et filtres par modules."),
        ("Automatisations", "Nouveau", "Workflows, triggers, conditions JSON et actions configurables."),
        ("Notifications", "Actif", "Notifications internes + options email/SMS/push dans les parametres."),
        ("Audit log", "Actif", "Historique des actions sensibles et exports."),
        ("Export / Import", "Nouveau", "Exports Excel existants + import CSV Clients/Produits."),
        ("Integrations tierces", "Nouveau", "Registre Zapier, Slack, Google, Microsoft, Stripe, webhooks et cles API referencees.")
    };

    private static List<(string Feature, string Status, string Details)> BuildAdvancedFeatureStatus() => new()
    {
        ("Reporting dynamique", "Nouveau", "Constructeur de rapports avec metriques, filtres JSON, partage et mode temps reel."),
        ("Analytics temps reel", "Actif", "Analytics existants + refresh mode RealTime pour rapports dynamiques."),
        ("IA integree", "Actif", "Assistant AI, CEO Dashboard, suggestions, priorites et previsions."),
        ("Collaboration", "Nouveau", "Commentaires, mentions et partage externe par entite metier."),
        ("Versioning des donnees", "Nouveau", "Snapshots JSON versionnes par entite pour historique et rollback futur."),
        ("Multi-tenant", "Nouveau", "Registre de tenants avec slug, plan et statut pour separation clients."),
        ("Marketplace d'extensions", "Nouveau", "Catalogue d'extensions installables et connecteurs metier."),
        ("Personnalisation profonde", "Nouveau", "Champs custom, themes, workflows custom et options JSON.")
    };
}
