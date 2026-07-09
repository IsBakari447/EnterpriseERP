using System.Text;
using EnterpriseERP.Services;

namespace EnterpriseERP.Middleware;

public class HtmlLocalizationMiddleware
{
    private readonly RequestDelegate _next;

    public HtmlLocalizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalBody = context.Response.Body;

        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await _next(context);

        context.Response.Body = originalBody;

        var contentType = context.Response.ContentType ?? string.Empty;
        if (!contentType.Contains("text/html", StringComparison.OrdinalIgnoreCase))
        {
            buffer.Position = 0;
            await buffer.CopyToAsync(originalBody);
            return;
        }

        buffer.Position = 0;
        using var reader = new StreamReader(buffer, Encoding.UTF8);
        var html = await reader.ReadToEndAsync();
        var lang = TranslationService.NormalizeLanguage(
            context.Session.GetString("Language")
            ?? context.Request.Cookies["Language"]);

        html = lang switch
        {
            "en" => Replace(html, En),
            "sv" => Replace(html, Sv),
            "es" => Replace(html, Es),
            "de" => Replace(html, De),
            _ => html
        };

        var bytes = Encoding.UTF8.GetBytes(html);
        context.Response.ContentLength = bytes.Length;
        await originalBody.WriteAsync(bytes);
    }

    private static string Replace(string html, IReadOnlyDictionary<string, string> map)
    {
        foreach (var item in map)
            html = html.Replace(item.Key, item.Value, StringComparison.OrdinalIgnoreCase);

        return html;
    }

    private static readonly Dictionary<string, string> En = new()
    {
        ["Ajouter"] = "Add",
        ["Modifier"] = "Edit",
        ["Supprimer"] = "Delete",
        ["Enregistrer"] = "Save",
        ["Annuler"] = "Cancel",
        ["Retour"] = "Back",
        ["Retour a la liste"] = "Back to list",
        ["Retour à la liste"] = "Back to list",
        ["Créer"] = "Create",
        ["Creer"] = "Create",
        ["Nouveau"] = "New",
        ["Nouvelle"] = "New",
        ["Aucun"] = "No",
        ["Aucune"] = "No",
        ["Rechercher"] = "Search",
        ["Exporter"] = "Export",
        ["Télécharger"] = "Download",
        ["Telecharger"] = "Download",
        ["Facture"] = "Invoice",
        ["Factures"] = "Invoices",
        ["Devis"] = "Quotes",
        ["Client"] = "Client",
        ["Clients"] = "Clients",
        ["Produit"] = "Product",
        ["Produits"] = "Products",
        ["Commande"] = "Order",
        ["Commandes"] = "Orders",
        ["Paiement"] = "Payment",
        ["Paiements"] = "Payments",
        ["Dépense"] = "Expense",
        ["Depense"] = "Expense",
        ["Dépenses"] = "Expenses",
        ["Depenses"] = "Expenses",
        ["Présence"] = "Attendance",
        ["Presence"] = "Attendance",
        ["Présences"] = "Attendance",
        ["Employé"] = "Employee",
        ["Employe"] = "Employee",
        ["Employés"] = "Employees",
        ["Fournisseur"] = "Supplier",
        ["Fournisseurs"] = "Suppliers",
        ["Paramètres"] = "Settings",
        ["Parametres"] = "Settings",
        ["Sécurité"] = "Security",
        ["Securite"] = "Security",
        ["Sauvegarde"] = "Backup",
        ["Téléphone"] = "Phone",
        ["Telephone"] = "Phone",
        ["Adresse"] = "Address",
        ["Nom complet"] = "Full name",
        ["Mot de passe"] = "Password",
        ["Rôle"] = "Role",
        ["Role"] = "Role",
        ["Statut"] = "Status",
        ["Montant"] = "Amount",
        ["Méthode"] = "Method",
        ["Methode"] = "Method",
        ["Description"] = "Description",
        ["Quantité"] = "Quantity",
        ["Quantite"] = "Quantity",
        ["Prix"] = "Price"
    };

    private static readonly Dictionary<string, string> Sv = new()
    {
        ["Ajouter"] = "Lagg till",
        ["Modifier"] = "Redigera",
        ["Supprimer"] = "Ta bort",
        ["Enregistrer"] = "Spara",
        ["Annuler"] = "Avbryt",
        ["Retour"] = "Tillbaka",
        ["Retour a la liste"] = "Tillbaka till listan",
        ["Retour à la liste"] = "Tillbaka till listan",
        ["Créer"] = "Skapa",
        ["Creer"] = "Skapa",
        ["Nouveau"] = "Ny",
        ["Nouvelle"] = "Ny",
        ["Aucun"] = "Ingen",
        ["Aucune"] = "Ingen",
        ["Rechercher"] = "Sok",
        ["Exporter"] = "Exportera",
        ["Télécharger"] = "Ladda ner",
        ["Telecharger"] = "Ladda ner",
        ["Facture"] = "Faktura",
        ["Factures"] = "Fakturor",
        ["Devis"] = "Offerter",
        ["Client"] = "Kund",
        ["Clients"] = "Kunder",
        ["Produit"] = "Produkt",
        ["Produits"] = "Produkter",
        ["Commande"] = "Order",
        ["Commandes"] = "Order",
        ["Paiement"] = "Betalning",
        ["Paiements"] = "Betalningar",
        ["Dépense"] = "Utgift",
        ["Depense"] = "Utgift",
        ["Dépenses"] = "Utgifter",
        ["Depenses"] = "Utgifter",
        ["Présence"] = "Narvaro",
        ["Presence"] = "Narvaro",
        ["Présences"] = "Narvaro",
        ["Employé"] = "Anstalld",
        ["Employe"] = "Anstalld",
        ["Employés"] = "Anstallda",
        ["Fournisseur"] = "Leverantor",
        ["Fournisseurs"] = "Leverantorer",
        ["Paramètres"] = "Installningar",
        ["Parametres"] = "Installningar",
        ["Sécurité"] = "Sakerhet",
        ["Securite"] = "Sakerhet",
        ["Sauvegarde"] = "Backup",
        ["Téléphone"] = "Telefon",
        ["Telephone"] = "Telefon",
        ["Adresse"] = "Adress",
        ["Nom complet"] = "Fullstandigt namn",
        ["Mot de passe"] = "Losenord",
        ["Rôle"] = "Roll",
        ["Role"] = "Roll",
        ["Statut"] = "Status",
        ["Montant"] = "Belopp",
        ["Méthode"] = "Metod",
        ["Methode"] = "Metod",
        ["Quantité"] = "Antal",
        ["Quantite"] = "Antal",
        ["Prix"] = "Pris"
    };

    private static readonly Dictionary<string, string> Es = new(En)
    {
        ["Add"] = "Agregar",
        ["Edit"] = "Editar",
        ["Delete"] = "Eliminar",
        ["Save"] = "Guardar",
        ["Cancel"] = "Cancelar",
        ["Back"] = "Volver",
        ["Create"] = "Crear",
        ["Search"] = "Buscar",
        ["Export"] = "Exportar",
        ["Download"] = "Descargar"
    };

    private static readonly Dictionary<string, string> De = new(En)
    {
        ["Add"] = "Hinzufugen",
        ["Edit"] = "Bearbeiten",
        ["Delete"] = "Loschen",
        ["Save"] = "Speichern",
        ["Cancel"] = "Abbrechen",
        ["Back"] = "Zuruck",
        ["Create"] = "Erstellen",
        ["Search"] = "Suchen",
        ["Export"] = "Exportieren",
        ["Download"] = "Herunterladen"
    };
}
