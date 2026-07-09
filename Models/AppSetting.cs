using System;
using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class AppSetting
    {
        [Key]
        public int Id { get; set; }

        // ==========================================================
        // ENTREPRISE
        // ==========================================================

        [Required]
        [StringLength(200)]
        public string CompanyName { get; set; } = "EnterpriseERP";

        public string? CompanyLogo { get; set; }

        public string? CompanyAddress { get; set; }

        public string? CompanyCity { get; set; }

        public string? CompanyCountry { get; set; }

        public string? CompanyPhone { get; set; }

        [EmailAddress]
        public string? CompanyEmail { get; set; }

        public string? CompanyWebsite { get; set; }

        public string? VatNumber { get; set; }

        public string? RegistrationNumber { get; set; }

        // ==========================================================
        // LOCALISATION
        // ==========================================================

        public string DefaultCurrency { get; set; } = "EUR";

        public string DefaultLanguage { get; set; } = "fr";

        public string TimeZone { get; set; } = "Europe/Stockholm";

        public string DateFormat { get; set; } = "dd/MM/yyyy";

        public string AmountFormat { get; set; } = "0.00";

        // ==========================================================
        // FINANCE
        // ==========================================================

        public decimal DefaultVatRate { get; set; } = 20;

        public bool VatIncludedByDefault { get; set; } = false;

        public string AccountingMode { get; set; } = "Simplified";

        // ==========================================================
        // FACTURATION
        // ==========================================================

        public string InvoicePrefix { get; set; } = "FAC";

        public int NextInvoiceNumber { get; set; } = 1;

        public string PaymentTerms { get; set; } = "Paiement à réception";

        public string DefaultThankYouMessage { get; set; } =
            "Merci beaucoup pour votre confiance. Nous serons ravis de vous accueillir à nouveau.";

        public string? InvoiceFooter { get; set; }

        public bool ShowLogoOnInvoice { get; set; } = true;

        public bool ShowQrCodeOnInvoice { get; set; } = false;

        // ==========================================================
        // STOCK
        // ==========================================================

        public int LowStockThreshold { get; set; } = 5;

        public bool AllowNegativeStock { get; set; } = false;

        public string StockValuationMethod { get; set; } = "FIFO";

        // ==========================================================
        // SECURITE
        // ==========================================================

        public int SessionTimeoutMinutes { get; set; } = 60;

        public bool ForcePasswordChange { get; set; } = false;

        public int PasswordExpirationDays { get; set; } = 90;

        public bool EnableTwoFactorAuth { get; set; } = false;

        public int MaxLoginAttempts { get; set; } = 5;

        public bool AutoLockAccounts { get; set; } = true;

        // ==========================================================
        // NOTIFICATIONS
        // ==========================================================

        public bool EnableEmailNotifications { get; set; } = false;

        public bool EnableSmsNotifications { get; set; } = false;

        public bool EnableInternalNotifications { get; set; } = true;

        public bool NotifyLowStock { get; set; } = true;

        public bool NotifyUnpaidInvoices { get; set; } = true;

        public bool NotifyPendingOrders { get; set; } = true;

        public bool NotifySecurityEvents { get; set; } = true;

        // ==========================================================
        // SAUVEGARDES
        // ==========================================================

        public bool EnableAutoBackup { get; set; } = false;

        public string BackupFrequency { get; set; } = "Daily";

        public int BackupRetentionDays { get; set; } = 30;

        public string? BackupPath { get; set; }

        // ==========================================================
        // APPARENCE
        // ==========================================================

        public string Theme { get; set; } = "Dark";

        public string PrimaryColor { get; set; } = "#2563eb";

        public string FontSize { get; set; } = "Medium";

        public string DashboardLayout { get; set; } = "Default";

        // ==========================================================
        // AUDIT
        // ==========================================================

        /// <summary>
        /// Date de création des paramètres
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Dernière modification.
        /// Nullable pour éviter les erreurs SQLite sur les anciennes données.
        /// </summary>
        public DateTime? UpdatedAt { get; set; } = DateTime.Now;
    }
}