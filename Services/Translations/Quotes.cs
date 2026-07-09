namespace EnterpriseERP.Services.Translations;

public static class Quotes
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Quotes"] = "Devis",
        ["QuotesDescription"] = "Gestion des devis clients.",
        ["AddQuote"] = "+ Nouveau devis",
        ["QuotesList"] = "Liste des devis",
        ["RegisteredQuotes"] = "devis",
        ["Quote"] = "Devis",
        ["QuoteNumber"] = "Numéro",
        ["ValidUntil"] = "Valable jusqu'au",
        ["AcceptedQuotes"] = "Devis acceptés",
        ["NoQuotes"] = "Aucun devis",
        ["NoQuotesText"] = "Aucun devis n'a encore été créé.",
        ["AddFirstQuote"] = "Créer le premier devis"
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Quotes"] = "Quotes",
        ["QuotesDescription"] = "Manage customer quotations.",
        ["AddQuote"] = "+ New quote",
        ["QuotesList"] = "Quotes list",
        ["RegisteredQuotes"] = "quote(s)",
        ["Quote"] = "Quote",
        ["QuoteNumber"] = "Number",
        ["ValidUntil"] = "Valid until",
        ["AcceptedQuotes"] = "Accepted quotes",
        ["NoQuotes"] = "No quotes",
        ["NoQuotesText"] = "No quotation has been created yet.",
        ["AddFirstQuote"] = "Create first quote"
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Quotes"] = "Offerter",
        ["QuotesDescription"] = "Hantera kundofferter.",
        ["AddQuote"] = "+ Ny offert",
        ["QuotesList"] = "Offertlista",
        ["RegisteredQuotes"] = "offert(er)",
        ["Quote"] = "Offert",
        ["QuoteNumber"] = "Nummer",
        ["ValidUntil"] = "Giltig till",
        ["AcceptedQuotes"] = "Accepterade offerter",
        ["NoQuotes"] = "Inga offerter",
        ["NoQuotesText"] = "Ingen offert har skapats ännu.",
        ["AddFirstQuote"] = "Skapa första offerten"
    };
}
