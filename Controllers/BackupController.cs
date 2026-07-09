using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace EnterpriseERP.Controllers
{
    public class BackupController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public BackupController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            IConfiguration configuration)
        {
            _context = context;
            _env = env;
            _configuration = configuration;
        }

        [RequirePermission("Paramètres", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            string backupFolder = GetBackupFolder();

            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);

            var backups = Directory.GetFiles(backupFolder, "*.zip")
                .Select(file => new FileInfo(file))
                .OrderByDescending(f => f.CreationTime)
                .ToList();

            return View(backups);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Paramètres", "Modifier")]
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            string dbPath = GetDatabasePath();

            if (!System.IO.File.Exists(dbPath))
            {
                TempData["Error"] = "Base de données introuvable.";
                return RedirectToAction(nameof(Index));
            }

            string backupFolder = GetBackupFolder();

            if (!Directory.Exists(backupFolder))
                Directory.CreateDirectory(backupFolder);

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string zipName = $"EnterpriseERP_Backup_{timestamp}.zip";
            string zipPath = Path.Combine(backupFolder, zipName);

            using (var zip = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                zip.CreateEntryFromFile(dbPath, Path.GetFileName(dbPath));
            }

            AuditService.Log(
                _context,
                HttpContext,
                "Sauvegardes",
                "Création",
                $"Sauvegarde créée : {zipName}",
                severity: "Information"
            );

            TempData["Success"] = "Sauvegarde créée avec succès.";

            return RedirectToAction(nameof(Index));
        }

        [RequirePermission("Paramètres", "Voir")]
        public IActionResult Download(string file)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(file))
                return NotFound();

            string backupFolder = GetBackupFolder();
            string safeFileName = Path.GetFileName(file);
            string filePath = Path.Combine(backupFolder, safeFileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            AuditService.Log(
                _context,
                HttpContext,
                "Sauvegardes",
                "Téléchargement",
                $"Sauvegarde téléchargée : {safeFileName}"
            );

            byte[] bytes = System.IO.File.ReadAllBytes(filePath);

            return File(bytes, "application/zip", safeFileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Paramètres", "Modifier")]
        public IActionResult Delete(string file)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(file))
                return RedirectToAction(nameof(Index));

            string backupFolder = GetBackupFolder();
            string safeFileName = Path.GetFileName(file);
            string filePath = Path.Combine(backupFolder, safeFileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

                AuditService.Log(
                    _context,
                    HttpContext,
                    "Sauvegardes",
                    "Suppression",
                    $"Sauvegarde supprimée : {safeFileName}",
                    severity: "Warning"
                );

                TempData["Success"] = "Sauvegarde supprimée.";
            }

            return RedirectToAction(nameof(Index));
        }

        private string GetBackupFolder()
        {
            return Path.Combine(_env.ContentRootPath, "Backups");
        }

        private string GetDatabasePath()
        {
            string? connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                return Path.Combine(_env.ContentRootPath, "enterpriseerp.db");

            string prefix = "Data Source=";

            if (connectionString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                string dbFile = connectionString.Substring(prefix.Length).Trim();

                if (Path.IsPathRooted(dbFile))
                    return dbFile;

                return Path.Combine(_env.ContentRootPath, dbFile);
            }

            return Path.Combine(_env.ContentRootPath, "enterpriseerp.db");
        }
    }
}