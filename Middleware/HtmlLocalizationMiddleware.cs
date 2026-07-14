using System.Text;
using System.Text.RegularExpressions;
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
        var output = new StringBuilder(html.Length);
        var insideRawBlock = false;

        foreach (Match match in Regex.Matches(html, @"<[^>]+>|[^<]+", RegexOptions.Singleline))
        {
            var chunk = match.Value;

            if (chunk.StartsWith('<'))
            {
                output.Append(insideRawBlock ? chunk : ReplaceTagAttributes(chunk, map));

                if (Regex.IsMatch(chunk, @"^<\s*(script|style)\b", RegexOptions.IgnoreCase))
                    insideRawBlock = true;
                else if (Regex.IsMatch(chunk, @"^<\s*/\s*(script|style)\s*>", RegexOptions.IgnoreCase))
                    insideRawBlock = false;

                continue;
            }

            output.Append(insideRawBlock ? chunk : ReplaceText(chunk, map));
        }

        return output.ToString();
    }

    private static string ReplaceText(string text, IReadOnlyDictionary<string, string> map)
    {
        foreach (var item in map.OrderByDescending(x => x.Key.Length))
            text = text.Replace(item.Key, item.Value, StringComparison.OrdinalIgnoreCase);

        return text;
    }

    private static string ReplaceTagAttributes(string tag, IReadOnlyDictionary<string, string> map)
    {
        return Regex.Replace(
            tag,
            @"\b(placeholder|title|aria-label)=""([^""]*)""",
            match =>
            {
                var value = ReplaceText(match.Groups[2].Value, map);
                return $@"{match.Groups[1].Value}=""{value}""";
            },
            RegexOptions.IgnoreCase);
    }

    private static readonly Dictionary<string, string> En = new()
    {
        ["Gestion centrale de la securite, des roles, des utilisateurs et des parametres ERP."] = "Central management of ERP security, roles, users and settings.",
        ["Configuration complète de l'entreprise, sécurité, facturation, stock et apparence"] = "Complete configuration of the company, security, invoicing, stock and appearance",
        ["Configuration complete de l'entreprise, securite, facturation, stock et apparence"] = "Complete configuration of the company, security, invoicing, stock and appearance",
        ["Surveillance des accès, utilisateurs, alertes et événements de sécurité"] = "Monitoring of access, users, alerts and security events",
        ["Surveillance des acces, utilisateurs, alertes et evenements de securite"] = "Monitoring of access, users, alerts and security events",
        ["Administration complète des droits d'accès de l'ERP"] = "Complete administration of ERP access rights",
        ["Administration complete des droits d'acces de l'ERP"] = "Complete administration of ERP access rights",
        ["Informations complètes du compte connecté"] = "Complete information for the signed-in account",
        ["Informations completes du compte connecte"] = "Complete information for the signed-in account",
        ["Mettre à jour vos informations personnelles"] = "Update your personal information",
        ["Mettre a jour vos informations personnelles"] = "Update your personal information",
        ["Gestion des utilisateurs"] = "User management",
        ["Gestion des rôles & permissions"] = "Roles & permissions management",
        ["Gestion des roles & permissions"] = "Roles & permissions management",
        ["Gestion des rôles"] = "Roles management",
        ["Gestion des roles"] = "Roles management",
        ["Retour Admin Center"] = "Back to Admin Center",
        ["Retour administration"] = "Back to administration",
        ["Retour dashboard"] = "Back to dashboard",
        ["Retour Dashboard"] = "Back to dashboard",
        ["Retour audit"] = "Back to audit",
        ["Retour profil"] = "Back to profile",
        ["Accès refusés récents"] = "Recent denied access",
        ["Acces refuses recents"] = "Recent denied access",
        ["Accès rapides sécurité"] = "Security quick actions",
        ["Acces rapides securite"] = "Security quick actions",
        ["Résumé utilisateurs"] = "Users summary",
        ["Resume utilisateurs"] = "Users summary",
        ["Derniers accès refusés"] = "Latest denied access",
        ["Derniers acces refuses"] = "Latest denied access",
        ["Dernières connexions"] = "Latest logins",
        ["Dernieres connexions"] = "Latest logins",
        ["Alertes de sécurité"] = "Security alerts",
        ["Alertes de securite"] = "Security alerts",
        ["Événements sécurité récents"] = "Recent security events",
        ["Evenements securite recents"] = "Recent security events",
        ["Informations système"] = "System information",
        ["Informations systeme"] = "System information",
        ["Base de données"] = "Database",
        ["Base de donnees"] = "Database",
        ["Dernière modification"] = "Last update",
        ["Derniere modification"] = "Last update",
        ["Dernière connexion"] = "Last login",
        ["Derniere connexion"] = "Last login",
        ["Dernière IP"] = "Last IP",
        ["Derniere IP"] = "Last IP",
        ["Compte approuvé"] = "Approved account",
        ["Compte approuve"] = "Approved account",
        ["Langue préférée"] = "Preferred language",
        ["Langue preferee"] = "Preferred language",
        ["Thème préféré"] = "Preferred theme",
        ["Theme prefere"] = "Preferred theme",
        ["Créé le"] = "Created on",
        ["Cree le"] = "Created on",
        ["Créer utilisateur"] = "Create user",
        ["Creer utilisateur"] = "Create user",
        ["Modifier profil"] = "Edit profile",
        ["Se déconnecter"] = "Sign out",
        ["Se deconnecter"] = "Sign out",
        ["Enregistrer les modifications"] = "Save changes",
        ["Enregistrer tous les paramètres"] = "Save all settings",
        ["Enregistrer tous les parametres"] = "Save all settings",
        ["Sauvegardes"] = "Backups",
        ["Sauvegarde automatique"] = "Automatic backup",
        ["Fréquence sauvegarde"] = "Backup frequency",
        ["Frequence sauvegarde"] = "Backup frequency",
        ["Sécurité utilisateurs"] = "User security",
        ["Securite utilisateurs"] = "User security",
        ["Paramètres sécurité"] = "Security settings",
        ["Parametres securite"] = "Security settings",
        ["Durée session en minutes"] = "Session duration in minutes",
        ["Duree session en minutes"] = "Session duration in minutes",
        ["Alerte sécurité"] = "Security alert",
        ["Alerte securite"] = "Security alert",
        ["Alerte factures impayées"] = "Unpaid invoices alert",
        ["Alerte factures impayees"] = "Unpaid invoices alert",
        ["Autoriser stock négatif"] = "Allow negative stock",
        ["Autoriser stock negatif"] = "Allow negative stock",
        ["Méthode valorisation stock"] = "Stock valuation method",
        ["Methode valorisation stock"] = "Stock valuation method",
        ["Préfixe facture"] = "Invoice prefix",
        ["Prefixe facture"] = "Invoice prefix",
        ["Prochain numéro facture"] = "Next invoice number",
        ["Prochain numero facture"] = "Next invoice number",
        ["TVA par défaut"] = "Default VAT",
        ["TVA par defaut"] = "Default VAT",
        ["TVA incluse par défaut"] = "VAT included by default",
        ["TVA incluse par defaut"] = "VAT included by default",
        ["Devise par défaut"] = "Default currency",
        ["Devise par defaut"] = "Default currency",
        ["Langue par défaut"] = "Default language",
        ["Langue par defaut"] = "Default language",
        ["Numéro TVA"] = "VAT number",
        ["Numero TVA"] = "VAT number",
        ["Numéro d'enregistrement"] = "Registration number",
        ["Numero d'enregistrement"] = "Registration number",
        ["Téléphone entreprise"] = "Company phone",
        ["Telephone entreprise"] = "Company phone",
        ["Recherche employés"] = "Employee search",
        ["Recherche employes"] = "Employee search",
        ["Recherche présences"] = "Attendance search",
        ["Recherche presences"] = "Attendance search",
        ["Résultats pour"] = "Results for",
        ["Resultats pour"] = "Results for",
        ["Catégorie"] = "Category",
        ["Categorie"] = "Category",
        ["Téléphone"] = "Phone",
        ["Presences"] = "Attendance",
        ["Employes"] = "Employees",
        ["Factures a suivre"] = "Invoices to follow",
        ["Aucun paiement enregistre."] = "No payment recorded.",
        ["Aucune depense enregistree."] = "No expense recorded.",
        ["Aucune facture recente."] = "No recent invoice.",
        ["Aucun accès refusé récent."] = "No recent denied access.",
        ["Aucun acces refuse recent."] = "No recent denied access.",
        ["Aucune connexion enregistrée."] = "No login recorded.",
        ["Aucune connexion enregistree."] = "No login recorded.",
        ["Aucune alerte de sécurité."] = "No security alert.",
        ["Aucune alerte de securite."] = "No security alert.",
        ["Aucun événement de sécurité récent."] = "No recent security event.",
        ["Aucun evenement de securite recent."] = "No recent security event.",
        ["Accès refusé"] = "Access denied",
        ["Acces refuse"] = "Access denied",
        ["Vous n'avez pas l'autorisation d'accéder à cette fonctionnalité."] = "You are not authorized to access this feature.",
        ["Vous n'avez pas l'autorisation d'acceder a cette fonctionnalite."] = "You are not authorized to access this feature.",
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
        ["Gestion centrale de la securite, des roles, des utilisateurs et des parametres ERP."] = "Central hantering av ERP-sakerhet, roller, anvandare och installningar.",
        ["Configuration complète de l'entreprise, sécurité, facturation, stock et apparence"] = "Fullstandig konfiguration av foretag, sakerhet, fakturering, lager och utseende",
        ["Configuration complete de l'entreprise, securite, facturation, stock et apparence"] = "Fullstandig konfiguration av foretag, sakerhet, fakturering, lager och utseende",
        ["Surveillance des accès, utilisateurs, alertes et événements de sécurité"] = "Overvakning av atkomst, anvandare, larm och sakerhetshandelser",
        ["Surveillance des acces, utilisateurs, alertes et evenements de securite"] = "Overvakning av atkomst, anvandare, larm och sakerhetshandelser",
        ["Administration complète des droits d'accès de l'ERP"] = "Fullstandig hantering av ERP-behorigheter",
        ["Administration complete des droits d'acces de l'ERP"] = "Fullstandig hantering av ERP-behorigheter",
        ["Informations complètes du compte connecté"] = "Fullstandig information om det inloggade kontot",
        ["Informations completes du compte connecte"] = "Fullstandig information om det inloggade kontot",
        ["Mettre à jour vos informations personnelles"] = "Uppdatera dina personuppgifter",
        ["Mettre a jour vos informations personnelles"] = "Uppdatera dina personuppgifter",
        ["Gestion des utilisateurs"] = "Anvandarhantering",
        ["Gestion des rôles & permissions"] = "Hantering av roller och behorigheter",
        ["Gestion des roles & permissions"] = "Hantering av roller och behorigheter",
        ["Gestion des rôles"] = "Rollhantering",
        ["Gestion des roles"] = "Rollhantering",
        ["Retour Admin Center"] = "Tillbaka till Admin Center",
        ["Retour administration"] = "Tillbaka till administration",
        ["Retour dashboard"] = "Tillbaka till dashboard",
        ["Retour Dashboard"] = "Tillbaka till dashboard",
        ["Retour audit"] = "Tillbaka till revision",
        ["Retour profil"] = "Tillbaka till profil",
        ["Accès refusés récents"] = "Senaste nekade atkomster",
        ["Acces refuses recents"] = "Senaste nekade atkomster",
        ["Accès rapides sécurité"] = "Snabblankar for sakerhet",
        ["Acces rapides securite"] = "Snabblankar for sakerhet",
        ["Résumé utilisateurs"] = "Anvandarsammanfattning",
        ["Resume utilisateurs"] = "Anvandarsammanfattning",
        ["Derniers accès refusés"] = "Senaste nekade atkomster",
        ["Derniers acces refuses"] = "Senaste nekade atkomster",
        ["Dernières connexions"] = "Senaste inloggningar",
        ["Dernieres connexions"] = "Senaste inloggningar",
        ["Alertes de sécurité"] = "Sakerhetslarm",
        ["Alertes de securite"] = "Sakerhetslarm",
        ["Événements sécurité récents"] = "Senaste sakerhetshandelser",
        ["Evenements securite recents"] = "Senaste sakerhetshandelser",
        ["Informations système"] = "Systeminformation",
        ["Informations systeme"] = "Systeminformation",
        ["Base de données"] = "Databas",
        ["Base de donnees"] = "Databas",
        ["Dernière modification"] = "Senaste andring",
        ["Derniere modification"] = "Senaste andring",
        ["Dernière connexion"] = "Senaste inloggning",
        ["Derniere connexion"] = "Senaste inloggning",
        ["Dernière IP"] = "Senaste IP",
        ["Derniere IP"] = "Senaste IP",
        ["Compte approuvé"] = "Godkant konto",
        ["Compte approuve"] = "Godkant konto",
        ["Langue préférée"] = "Foredraget sprak",
        ["Langue preferee"] = "Foredraget sprak",
        ["Thème préféré"] = "Foredraget tema",
        ["Theme prefere"] = "Foredraget tema",
        ["Créé le"] = "Skapad den",
        ["Cree le"] = "Skapad den",
        ["Créer utilisateur"] = "Skapa anvandare",
        ["Creer utilisateur"] = "Skapa anvandare",
        ["Modifier profil"] = "Redigera profil",
        ["Se déconnecter"] = "Logga ut",
        ["Se deconnecter"] = "Logga ut",
        ["Enregistrer les modifications"] = "Spara andringar",
        ["Enregistrer tous les paramètres"] = "Spara alla installningar",
        ["Enregistrer tous les parametres"] = "Spara alla installningar",
        ["Sauvegardes"] = "Backuper",
        ["Sauvegarde automatique"] = "Automatisk backup",
        ["Fréquence sauvegarde"] = "Backupfrekvens",
        ["Frequence sauvegarde"] = "Backupfrekvens",
        ["Sécurité utilisateurs"] = "Anvandarsakerhet",
        ["Securite utilisateurs"] = "Anvandarsakerhet",
        ["Paramètres sécurité"] = "Sakerhetsinstallningar",
        ["Parametres securite"] = "Sakerhetsinstallningar",
        ["Durée session en minutes"] = "Sessionstid i minuter",
        ["Duree session en minutes"] = "Sessionstid i minuter",
        ["Alerte sécurité"] = "Sakerhetslarm",
        ["Alerte securite"] = "Sakerhetslarm",
        ["Alerte factures impayées"] = "Larm for obetalda fakturor",
        ["Alerte factures impayees"] = "Larm for obetalda fakturor",
        ["Autoriser stock négatif"] = "Tillat negativt lager",
        ["Autoriser stock negatif"] = "Tillat negativt lager",
        ["Méthode valorisation stock"] = "Metod for lagervardering",
        ["Methode valorisation stock"] = "Metod for lagervardering",
        ["Préfixe facture"] = "Fakturaprefix",
        ["Prefixe facture"] = "Fakturaprefix",
        ["Prochain numéro facture"] = "Nasta fakturanummer",
        ["Prochain numero facture"] = "Nasta fakturanummer",
        ["TVA par défaut"] = "Standardmoms",
        ["TVA par defaut"] = "Standardmoms",
        ["TVA incluse par défaut"] = "Moms inkluderad som standard",
        ["TVA incluse par defaut"] = "Moms inkluderad som standard",
        ["Devise par défaut"] = "Standardvaluta",
        ["Devise par defaut"] = "Standardvaluta",
        ["Langue par défaut"] = "Standardsprak",
        ["Langue par defaut"] = "Standardsprak",
        ["Numéro TVA"] = "Momsnummer",
        ["Numero TVA"] = "Momsnummer",
        ["Numéro d'enregistrement"] = "Registreringsnummer",
        ["Numero d'enregistrement"] = "Registreringsnummer",
        ["Téléphone entreprise"] = "Foretagstelefon",
        ["Telephone entreprise"] = "Foretagstelefon",
        ["Recherche employés"] = "Sok anstallda",
        ["Recherche employes"] = "Sok anstallda",
        ["Recherche présences"] = "Sok narvaro",
        ["Recherche presences"] = "Sok narvaro",
        ["Résultats pour"] = "Resultat for",
        ["Resultats pour"] = "Resultat for",
        ["Catégorie"] = "Kategori",
        ["Categorie"] = "Kategori",
        ["Téléphone"] = "Telefon",
        ["Presences"] = "Narvaro",
        ["Employes"] = "Anstallda",
        ["Factures a suivre"] = "Fakturor att folja",
        ["Aucun paiement enregistre."] = "Ingen betalning registrerad.",
        ["Aucune depense enregistree."] = "Ingen utgift registrerad.",
        ["Aucune facture recente."] = "Ingen ny faktura.",
        ["Aucun accès refusé récent."] = "Ingen nekad atkomst nyligen.",
        ["Aucun acces refuse recent."] = "Ingen nekad atkomst nyligen.",
        ["Aucune connexion enregistrée."] = "Ingen inloggning registrerad.",
        ["Aucune connexion enregistree."] = "Ingen inloggning registrerad.",
        ["Aucune alerte de sécurité."] = "Inget sakerhetslarm.",
        ["Aucune alerte de securite."] = "Inget sakerhetslarm.",
        ["Aucun événement de sécurité récent."] = "Ingen sakerhetshandelse nyligen.",
        ["Aucun evenement de securite recent."] = "Ingen sakerhetshandelse nyligen.",
        ["Accès refusé"] = "Atkomst nekad",
        ["Acces refuse"] = "Atkomst nekad",
        ["Vous n'avez pas l'autorisation d'accéder à cette fonctionnalité."] = "Du har inte behorighet att komma at den har funktionen.",
        ["Vous n'avez pas l'autorisation d'acceder a cette fonctionnalite."] = "Du har inte behorighet att komma at den har funktionen.",
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
