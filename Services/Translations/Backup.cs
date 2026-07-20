namespace EnterpriseERP.Services.Translations;

public static class Backup
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Backup"] = "Backup",
        ["BackupCenter"] = "Centre de sauvegarde",
        ["BackupDescription"] = "Sauvegarde manuelle de la base de donnees EnterpriseERP",
        ["BackToAdminCenter"] = "Retour au centre d'administration",
        ["CreateBackup"] = "Creer une sauvegarde",
        ["CreateBackupDescription"] = "Cette action cree un fichier ZIP contenant la base SQLite actuelle.",
        ["CreateBackupNow"] = "Creer une sauvegarde maintenant",
        ["AvailableBackups"] = "Sauvegardes disponibles",
        ["File"] = "Fichier",
        ["Size"] = "Taille",
        ["Download"] = "Telecharger",
        ["ConfirmDeleteBackup"] = "Supprimer cette sauvegarde ?",
        ["NoBackups"] = "Aucune sauvegarde disponible.",
        ["DatabaseNotFound"] = "Base de donnees introuvable.",
        ["BackupCreatedSuccess"] = "Sauvegarde creee avec succes.",
        ["BackupDeletedSuccess"] = "Sauvegarde supprimee."
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Backup"] = "Backup",
        ["BackupCenter"] = "Backup Center",
        ["BackupDescription"] = "Manual backup of the EnterpriseERP database",
        ["BackToAdminCenter"] = "Back to Admin Center",
        ["CreateBackup"] = "Create a backup",
        ["CreateBackupDescription"] = "This action creates a ZIP file containing the current SQLite database.",
        ["CreateBackupNow"] = "Create a backup now",
        ["AvailableBackups"] = "Available backups",
        ["File"] = "File",
        ["Size"] = "Size",
        ["Download"] = "Download",
        ["ConfirmDeleteBackup"] = "Delete this backup?",
        ["NoBackups"] = "No backups available.",
        ["DatabaseNotFound"] = "Database not found.",
        ["BackupCreatedSuccess"] = "Backup created successfully.",
        ["BackupDeletedSuccess"] = "Backup deleted."
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Backup"] = "Säkerhetskopiering",
        ["BackupCenter"] = "Säkerhetskopieringscenter",
        ["BackupDescription"] = "Manuell säkerhetskopiering av EnterpriseERP-databasen",
        ["BackToAdminCenter"] = "Tillbaka till admincentret",
        ["CreateBackup"] = "Skapa en säkerhetskopia",
        ["CreateBackupDescription"] = "Den här åtgärden skapar en ZIP-fil som innehåller den aktuella SQLite-databasen.",
        ["CreateBackupNow"] = "Skapa en säkerhetskopia nu",
        ["AvailableBackups"] = "Tillgängliga säkerhetskopior",
        ["File"] = "Fil",
        ["Size"] = "Storlek",
        ["Download"] = "Ladda ner",
        ["ConfirmDeleteBackup"] = "Vill du ta bort den här säkerhetskopian?",
        ["NoBackups"] = "Ingen säkerhetskopia tillgänglig.",
        ["DatabaseNotFound"] = "Databasen hittades inte.",
        ["BackupCreatedSuccess"] = "Säkerhetskopian skapades.",
        ["BackupDeletedSuccess"] = "Säkerhetskopian togs bort."
    };
}
