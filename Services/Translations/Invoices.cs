namespace EnterpriseERP.Services.Translations;

public static class Invoices
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Invoices"] = "Factures",
        ["InvoicesDescription"] = "Gestion des factures et paiements.",
        ["AddInvoice"] = "+ Nouvelle facture",
        ["InvoicesList"] = "Liste des factures",
        ["RegisteredInvoices"] = "facture(s)",
        ["Details"] = "Détails",
        ["Preview"] = "Aperçu",
        ["Pending"] = "En attente",
        ["Paid"] = "Payées",
        ["Unpaid"] = "Impayées",
        ["NoInvoices"] = "Aucune facture",
        ["NoInvoicesText"] = "Aucune facture n'a encore été enregistrée.",
        ["AddFirstInvoice"] = "Créer la première facture",
        ["UnpaidInvoices"] = "Factures impayées",
        ["PaidUnpaidInvoices"] = "Factures payées / impayées"
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Invoices"] = "Invoices",
        ["InvoicesDescription"] = "Manage invoices and payments.",
        ["AddInvoice"] = "+ New invoice",
        ["InvoicesList"] = "Invoices list",
        ["RegisteredInvoices"] = "invoice(s)",
        ["Details"] = "Details",
        ["Preview"] = "Preview",
        ["Pending"] = "Pending",
        ["Paid"] = "Paid",
        ["Unpaid"] = "Unpaid",
        ["NoInvoices"] = "No invoices",
        ["NoInvoicesText"] = "No invoice has been created yet.",
        ["AddFirstInvoice"] = "Create first invoice",
        ["UnpaidInvoices"] = "Unpaid invoices",
        ["PaidUnpaidInvoices"] = "Paid / unpaid invoices"
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Invoices"] = "Fakturor",
        ["InvoicesDescription"] = "Hantera fakturor och betalningar.",
        ["AddInvoice"] = "+ Ny faktura",
        ["InvoicesList"] = "Fakturalista",
        ["RegisteredInvoices"] = "faktura(or)",
        ["Details"] = "Detaljer",
        ["Preview"] = "Förhandsvisning",
        ["Pending"] = "Väntande",
        ["Paid"] = "Betalda",
        ["Unpaid"] = "Obetalda",
        ["NoInvoices"] = "Inga fakturor",
        ["NoInvoicesText"] = "Ingen faktura har skapats ännu.",
        ["AddFirstInvoice"] = "Skapa första fakturan",
        ["UnpaidInvoices"] = "Obetalda fakturor",
        ["PaidUnpaidInvoices"] = "Betalda / obetalda fakturor"
    };
}
