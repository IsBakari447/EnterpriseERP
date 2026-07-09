namespace EnterpriseERP.Services.Translations;

public static class Orders
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Orders"] = "Commandes",
        ["OrdersDescription"] = "Gestion des commandes clients.",
        ["AddOrder"] = "+ Nouvelle commande",
        ["OrdersList"] = "Liste des commandes",
        ["RegisteredOrders"] = "commande(s)",
        ["NoOrders"] = "Aucune commande",
        ["NoOrdersText"] = "Aucune commande n'a encore été enregistrée.",
        ["AddFirstOrder"] = "Créer la première commande",
        ["PendingOrders"] = "Commandes en attente",
        ["MonthlyOrders"] = "Commandes par mois"
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Orders"] = "Orders",
        ["OrdersDescription"] = "Manage customer orders.",
        ["AddOrder"] = "+ New order",
        ["OrdersList"] = "Orders list",
        ["RegisteredOrders"] = "order(s)",
        ["NoOrders"] = "No orders",
        ["NoOrdersText"] = "No order has been created yet.",
        ["AddFirstOrder"] = "Create first order",
        ["PendingOrders"] = "Pending orders",
        ["MonthlyOrders"] = "Monthly orders"
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Orders"] = "Beställningar",
        ["OrdersDescription"] = "Hantera kundbeställningar.",
        ["AddOrder"] = "+ Ny beställning",
        ["OrdersList"] = "Beställningslista",
        ["RegisteredOrders"] = "beställning(ar)",
        ["NoOrders"] = "Inga beställningar",
        ["NoOrdersText"] = "Ingen beställning har skapats ännu.",
        ["AddFirstOrder"] = "Skapa första beställningen",
        ["PendingOrders"] = "Väntande beställningar",
        ["MonthlyOrders"] = "Beställningar per månad"
    };
}
