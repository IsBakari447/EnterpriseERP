namespace EnterpriseERP.Services.Translations;

public static class Clients
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Clients"] = "Clients",
        ["ClientsDescription"] = "Gestion des clients et des entreprises.",
        ["AddClient"] = "+ Ajouter un client",
        ["ClientsList"] = "Liste des clients",
        ["RegisteredClients"] = "client(s)",
        ["Company"] = "Entreprise",
        ["NoClients"] = "Aucun client",
        ["NoClientsText"] = "Aucun client n'a encore été enregistré.",
        ["AddFirstClient"] = "Créer le premier client"
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Clients"] = "Clients",
        ["ClientsDescription"] = "Manage customers and companies.",
        ["AddClient"] = "+ Add client",
        ["ClientsList"] = "Customers list",
        ["RegisteredClients"] = "customer(s)",
        ["Company"] = "Company",
        ["NoClients"] = "No customers",
        ["NoClientsText"] = "No customer has been created yet.",
        ["AddFirstClient"] = "Create first customer"
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Clients"] = "Kunder",
        ["ClientsDescription"] = "Hantera kunder och företag.",
        ["AddClient"] = "+ Lägg till kund",
        ["ClientsList"] = "Kundlista",
        ["RegisteredClients"] = "kund(er)",
        ["Company"] = "Företag",
        ["NoClients"] = "Inga kunder",
        ["NoClientsText"] = "Det finns inga registrerade kunder.",
        ["AddFirstClient"] = "Skapa första kunden"
    };
}
