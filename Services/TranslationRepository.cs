using EnterpriseERP.Services.Translations;

namespace EnterpriseERP.Services;

public static class TranslationRepository
{
    public static readonly Dictionary<string,string> French = new();
    public static readonly Dictionary<string,string> English = new();
    public static readonly Dictionary<string,string> Swedish = new();
    public static readonly Dictionary<string,string> Spanish = new();
    public static readonly Dictionary<string,string> German = new();

    static TranslationRepository()
    {
        Add(French, Common.Fr);
        Add(French, Dashboard.Fr);
        Add(French, Clients.Fr);
        Add(French, Products.Fr);
        Add(French, Suppliers.Fr);
        Add(French, Quotes.Fr);
        Add(French, Orders.Fr);
        Add(French, Invoices.Fr);
        Add(French, Payments.Fr);
        Add(French, Expenses.Fr);
        Add(French, Employees.Fr);
        Add(French, Presence.Fr);
        Add(French, Audit.Fr);
        Add(French, Backup.Fr);
        Add(French, Security.Fr);
        Add(French, Settings.Fr);
        Add(French, Account.Fr);
        Add(French, Employees.Fr);
        Add(French, Presence.Fr);

        Add(English, Common.En);
        Add(English, Dashboard.En);
        Add(English, Clients.En);
        Add(English, Products.En);
        Add(English, Suppliers.En);
        Add(English, Quotes.En);
        Add(English, Orders.En);
        Add(English, Invoices.En);
        Add(English, Payments.En);
        Add(English, Expenses.En);
        Add(English, Employees.En);
        Add(English, Presence.En);
        Add(English, Audit.En);
        Add(English, Backup.En);
        Add(English, Security.En);
        Add(English, Settings.En);
        Add(English, Account.En);
        Add(English, Employees.En);
        Add(English, Presence.En);

        Add(Swedish, Common.Sv);
        Add(Swedish, Dashboard.Sv);
        Add(Swedish, Clients.Sv);
        Add(Swedish, Products.Sv);
        Add(Swedish, Suppliers.Sv);
        Add(Swedish, Quotes.Sv);
        Add(Swedish, Orders.Sv);
        Add(Swedish, Invoices.Sv);
        Add(Swedish, Payments.Sv);
        Add(Swedish, Expenses.Sv);
        Add(Swedish, Employees.Sv);
        Add(Swedish, Presence.Sv);
        Add(Swedish, Audit.Sv);
        Add(Swedish, Backup.Sv);
        Add(Swedish, Security.Sv);
        Add(Swedish, Settings.Sv);
        Add(Swedish, Account.Sv);
        Add(Swedish, Employees.Sv);
        Add(Swedish, Presence.Sv);

        Add(Spanish, French);
        Add(Spanish, Common.Es);
        Add(Spanish, Dashboard.Es);
        Add(Spanish, Account.Es);

        Add(German, French);
        Add(German, Common.De);
        Add(German, Dashboard.De);
        Add(German, Account.De);
    }

    private static void Add(Dictionary<string,string> destination, Dictionary<string,string> source)
    {
        foreach (var item in source)
        {
            destination[item.Key] = item.Value;
        }
    }
}
