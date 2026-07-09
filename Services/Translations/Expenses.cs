namespace EnterpriseERP.Services.Translations;

public static class Expenses
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Expenses"] = "Dépenses",
        ["ExpensesDescription"] = "Suivi des dépenses de l'entreprise.",
        ["AddExpense"] = "+ Nouvelle dépense",
        ["ExpensesList"] = "Liste des dépenses",
        ["RegisteredExpenses"] = "dépense(s)",
        ["Title"] = "Titre",
        ["PaymentMethod"] = "Moyen de paiement",
        ["CreatedBy"] = "Créé par",
        ["NoExpenses"] = "Aucune dépense",
        ["NoExpensesText"] = "Aucune dépense n'a encore été enregistrée.",
        ["AddFirstExpense"] = "Créer la première dépense",
        ["ConfirmDeleteExpense"] = "Supprimer cette dépense ?"
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Expenses"] = "Expenses",
        ["ExpensesDescription"] = "Track company expenses.",
        ["AddExpense"] = "+ New expense",
        ["ExpensesList"] = "Expenses list",
        ["RegisteredExpenses"] = "expense(s)",
        ["Title"] = "Title",
        ["PaymentMethod"] = "Payment method",
        ["CreatedBy"] = "Created by",
        ["NoExpenses"] = "No expenses",
        ["NoExpensesText"] = "No expense has been created yet.",
        ["AddFirstExpense"] = "Create first expense",
        ["ConfirmDeleteExpense"] = "Delete this expense?"
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Expenses"] = "Utgifter",
        ["ExpensesDescription"] = "Hantera företagets utgifter.",
        ["AddExpense"] = "+ Ny utgift",
        ["ExpensesList"] = "Utgiftslista",
        ["RegisteredExpenses"] = "utgift(er)",
        ["Title"] = "Titel",
        ["PaymentMethod"] = "Betalningsmetod",
        ["CreatedBy"] = "Skapad av",
        ["NoExpenses"] = "Inga utgifter",
        ["NoExpensesText"] = "Ingen utgift har skapats ännu.",
        ["AddFirstExpense"] = "Skapa första utgiften",
        ["ConfirmDeleteExpense"] = "Ta bort denna utgift?"
    };
}
