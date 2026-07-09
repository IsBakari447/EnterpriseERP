namespace EnterpriseERP.Services.Translations;

public static class Presence
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Presences"] = "Présences",
        ["PresencesDescription"] = "Suivi des entrées et sorties des employés.",
        ["CheckIn"] = "Pointer entrée",
        ["CheckOut"] = "Pointer sortie",
        ["PresencesHistory"] = "Historique des présences",
        ["RegisteredPresences"] = "présence(s)",
        ["CheckInTime"] = "Entrée",
        ["CheckOutTime"] = "Sortie",
        ["Completed"] = "Terminé",
        ["InProgress"] = "En cours",
        ["Absent"] = "Absent",
        ["NoPresences"] = "Aucune présence",
        ["NoPresencesText"] = "Aucune présence n'a encore été enregistrée."
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Presences"] = "Attendance",
        ["PresencesDescription"] = "Track employee check-ins and check-outs.",
        ["CheckIn"] = "Check in",
        ["CheckOut"] = "Check out",
        ["PresencesHistory"] = "Attendance history",
        ["RegisteredPresences"] = "attendance record(s)",
        ["CheckInTime"] = "Check-in",
        ["CheckOutTime"] = "Check-out",
        ["Completed"] = "Completed",
        ["InProgress"] = "In progress",
        ["Absent"] = "Absent",
        ["NoPresences"] = "No attendance records",
        ["NoPresencesText"] = "No attendance record has been created yet."
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Presences"] = "Närvaro",
        ["PresencesDescription"] = "Följ anställdas in- och utcheckningar.",
        ["CheckIn"] = "Checka in",
        ["CheckOut"] = "Checka ut",
        ["PresencesHistory"] = "Närvarohistorik",
        ["RegisteredPresences"] = "närvaropost(er)",
        ["CheckInTime"] = "Incheckning",
        ["CheckOutTime"] = "Utcheckning",
        ["Completed"] = "Klar",
        ["InProgress"] = "Pågår",
        ["Absent"] = "Frånvarande",
        ["NoPresences"] = "Ingen närvaro",
        ["NoPresencesText"] = "Ingen närvaropost har skapats ännu."
    };
}
