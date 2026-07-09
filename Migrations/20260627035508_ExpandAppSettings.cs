using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseERP.Migrations
{
    /// <inheritdoc />
    public partial class ExpandAppSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Phone",
                table: "AppSettings",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "AppSettings",
                newName: "TimeZone");

            migrationBuilder.RenameColumn(
                name: "Currency",
                table: "AppSettings",
                newName: "Theme");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "AppSettings",
                newName: "StockValuationMethod");

            migrationBuilder.AddColumn<string>(
                name: "AccountingMode",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AllowNegativeStock",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AmountFormat",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "AutoLockAccounts",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BackupFrequency",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BackupPath",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BackupRetentionDays",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CompanyAddress",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCity",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyCountry",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyEmail",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyLogo",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyPhone",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyWebsite",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DashboardLayout",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DateFormat",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultCurrency",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DefaultThankYouMessage",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DefaultVatRate",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "EnableAutoBackup",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableEmailNotifications",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableInternalNotifications",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableSmsNotifications",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "EnableTwoFactorAuth",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "FontSize",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ForcePasswordChange",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceFooter",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoicePrefix",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MaxLoginAttempts",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NextInvoiceNumber",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyLowStock",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyPendingOrders",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifySecurityEvents",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotifyUnpaidInvoices",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PasswordExpirationDays",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PaymentTerms",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                table: "AppSettings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionTimeoutMinutes",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowLogoOnInvoice",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowQrCodeOnInvoice",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "VatIncludedByDefault",
                table: "AppSettings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VatNumber",
                table: "AppSettings",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountingMode",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "AllowNegativeStock",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "AmountFormat",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "AutoLockAccounts",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "BackupFrequency",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "BackupPath",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "BackupRetentionDays",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "CompanyAddress",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "CompanyCity",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "CompanyCountry",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "CompanyEmail",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "CompanyLogo",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "CompanyPhone",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "CompanyWebsite",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "DashboardLayout",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "DateFormat",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "DefaultCurrency",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "DefaultThankYouMessage",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "DefaultVatRate",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "EnableAutoBackup",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "EnableEmailNotifications",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "EnableInternalNotifications",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "EnableSmsNotifications",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "EnableTwoFactorAuth",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "ForcePasswordChange",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "InvoiceFooter",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "InvoicePrefix",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "MaxLoginAttempts",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "NextInvoiceNumber",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "NotifyLowStock",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "NotifyPendingOrders",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "NotifySecurityEvents",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "NotifyUnpaidInvoices",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "PasswordExpirationDays",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "PaymentTerms",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "PrimaryColor",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "SessionTimeoutMinutes",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "ShowLogoOnInvoice",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "ShowQrCodeOnInvoice",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "VatIncludedByDefault",
                table: "AppSettings");

            migrationBuilder.DropColumn(
                name: "VatNumber",
                table: "AppSettings");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "AppSettings",
                newName: "Phone");

            migrationBuilder.RenameColumn(
                name: "TimeZone",
                table: "AppSettings",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "Theme",
                table: "AppSettings",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "StockValuationMethod",
                table: "AppSettings",
                newName: "Address");
        }
    }
}
