using EnterpriseERP.Attributes;
using EnterpriseERP.Data;
using EnterpriseERP.Models;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers
{
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------------------------------------------------------
        // 🔥 Changer la langue (bouton ajouté)
        // ---------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeLanguage(string language)
        {
            language = TranslationService.NormalizeLanguage(language);

            HttpContext.Session.SetString("Language", language);
            Response.Cookies.Append("Language", language, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Path = "/",
                SameSite = SameSiteMode.Lax,
                HttpOnly = false
            });

            return Redirect(Request.Headers["Referer"].ToString());
        }

        // ---------------------------------------------------------
        // 🔥 Page principale des paramètres
        // ---------------------------------------------------------
        [RequirePermission("Paramètres", "Voir")]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var settings = GetOrCreateSettings();

            return View(settings);
        }

        // ---------------------------------------------------------
        // 🔥 Sauvegarde des paramètres
        // ---------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequirePermission("Paramètres", "Modifier")]
        public IActionResult Index(AppSetting model)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
                return RedirectToAction("Login", "Account");

            var settings = GetOrCreateSettings();

            settings.CompanyName = model.CompanyName;
            settings.CompanyLogo = model.CompanyLogo;
            settings.CompanyAddress = model.CompanyAddress ?? "";
            settings.CompanyCity = model.CompanyCity;
            settings.CompanyCountry = model.CompanyCountry;
            settings.CompanyPhone = model.CompanyPhone;
            settings.CompanyEmail = model.CompanyEmail;
            settings.CompanyWebsite = model.CompanyWebsite;
            settings.VatNumber = model.VatNumber;
            settings.RegistrationNumber = model.RegistrationNumber;

            settings.DefaultCurrency = model.DefaultCurrency;
            settings.DefaultLanguage = model.DefaultLanguage;
            settings.TimeZone = model.TimeZone;
            settings.DateFormat = model.DateFormat;
            settings.AmountFormat = model.AmountFormat;

            settings.DefaultVatRate = model.DefaultVatRate;
            settings.VatIncludedByDefault = model.VatIncludedByDefault;
            settings.AccountingMode = model.AccountingMode;

            settings.InvoicePrefix = model.InvoicePrefix;
            settings.NextInvoiceNumber = model.NextInvoiceNumber;
            settings.PaymentTerms = model.PaymentTerms;
            settings.DefaultThankYouMessage = model.DefaultThankYouMessage;
            settings.InvoiceFooter = model.InvoiceFooter;
            settings.ShowLogoOnInvoice = model.ShowLogoOnInvoice;
            settings.ShowQrCodeOnInvoice = model.ShowQrCodeOnInvoice;

            settings.LowStockThreshold = model.LowStockThreshold;
            settings.AllowNegativeStock = model.AllowNegativeStock;
            settings.StockValuationMethod = model.StockValuationMethod;

            settings.SessionTimeoutMinutes = model.SessionTimeoutMinutes;
            settings.ForcePasswordChange = model.ForcePasswordChange;
            settings.PasswordExpirationDays = model.PasswordExpirationDays;
            settings.EnableTwoFactorAuth = model.EnableTwoFactorAuth;
            settings.MaxLoginAttempts = model.MaxLoginAttempts;
            settings.AutoLockAccounts = model.AutoLockAccounts;

            settings.EnableEmailNotifications = model.EnableEmailNotifications;
            settings.EnableSmsNotifications = model.EnableSmsNotifications;
            settings.EnableInternalNotifications = model.EnableInternalNotifications;
            settings.NotifyLowStock = model.NotifyLowStock;
            settings.NotifyUnpaidInvoices = model.NotifyUnpaidInvoices;
            settings.NotifyPendingOrders = model.NotifyPendingOrders;
            settings.NotifySecurityEvents = model.NotifySecurityEvents;

            settings.EnableAutoBackup = model.EnableAutoBackup;
            settings.BackupFrequency = model.BackupFrequency;
            settings.BackupRetentionDays = model.BackupRetentionDays;
            settings.BackupPath = model.BackupPath;

            settings.Theme = model.Theme;
            settings.PrimaryColor = model.PrimaryColor;
            settings.FontSize = model.FontSize;
            settings.DashboardLayout = model.DashboardLayout;

            settings.UpdatedAt = DateTime.Now;

            _context.SaveChanges();

            AuditService.Log(
                _context,
                HttpContext,
                "Paramètres",
                "Modification",
                "Settings Center mis à jour",
                severity: "Warning"
            );

            ViewBag.Success = "Paramètres enregistrés avec succès.";

            return View(settings);
        }

        // ---------------------------------------------------------
        // 🔥 Création automatique des paramètres si absents
        // ---------------------------------------------------------
        private AppSetting GetOrCreateSettings()
        {
            var settings = _context.AppSettings.FirstOrDefault();

            if (settings == null)
            {
                settings = new AppSetting
                {
                    CompanyName = "EnterpriseERP",
                    CompanyAddress = "",
                    DefaultCurrency = "EUR",
                    DefaultLanguage = "fr",
                    LowStockThreshold = 5
                };

                _context.AppSettings.Add(settings);
                _context.SaveChanges();
            }

            return settings;
        }
    }
}
