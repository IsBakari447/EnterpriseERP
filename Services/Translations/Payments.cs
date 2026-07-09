namespace EnterpriseERP.Services.Translations;

public static class Payments
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Payments"] = "Paiements",
        ["PaymentsDescription"] = "Gestion des paiements et règlements.",
        ["AddPayment"] = "+ Ajouter un paiement",
        ["PaymentsHistory"] = "Historique des paiements",
        ["RegisteredPayments"] = "paiement(s)",
        ["Reference"] = "Référence",
        ["NoPayments"] = "Aucun paiement",
        ["NoPaymentsText"] = "Aucun paiement n'a encore été enregistré.",
        ["AddFirstPayment"] = "Créer le premier paiement",
        ["RecentPayments"] = "Paiements récents",
        ["PaymentsByMethod"] = "Paiements par méthode"
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Payments"] = "Payments",
        ["PaymentsDescription"] = "Manage payments and settlements.",
        ["AddPayment"] = "+ Add payment",
        ["PaymentsHistory"] = "Payment history",
        ["RegisteredPayments"] = "payment(s)",
        ["Reference"] = "Reference",
        ["NoPayments"] = "No payments",
        ["NoPaymentsText"] = "No payment has been created yet.",
        ["AddFirstPayment"] = "Create first payment",
        ["RecentPayments"] = "Recent payments",
        ["PaymentsByMethod"] = "Payments by method"
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Payments"] = "Betalningar",
        ["PaymentsDescription"] = "Hantera betalningar och regleringar.",
        ["AddPayment"] = "+ Lägg till betalning",
        ["PaymentsHistory"] = "Betalningshistorik",
        ["RegisteredPayments"] = "betalning(ar)",
        ["Reference"] = "Referens",
        ["NoPayments"] = "Inga betalningar",
        ["NoPaymentsText"] = "Ingen betalning har skapats ännu.",
        ["AddFirstPayment"] = "Skapa första betalningen",
        ["RecentPayments"] = "Senaste betalningar",
        ["PaymentsByMethod"] = "Betalningar per metod"
    };
}
