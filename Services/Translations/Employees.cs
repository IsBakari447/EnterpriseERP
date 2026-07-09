namespace EnterpriseERP.Services.Translations;

public static class Employees
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Employees"] = "Employés",
        ["EmployeesDescription"] = "Gestion des employés de l'entreprise.",
        ["AddEmployee"] = "+ Ajouter un employé",
        ["EmployeesList"] = "Liste des employés",
        ["RegisteredEmployees"] = "employé(s)",
        ["Employee"] = "Employé",
        ["Position"] = "Poste",
        ["Salary"] = "Salaire",
        ["NoEmployees"] = "Aucun employé",
        ["NoEmployeesText"] = "Aucun employé n'a encore été enregistré.",
        ["AddFirstEmployee"] = "Créer le premier employé"
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Employees"] = "Employees",
        ["EmployeesDescription"] = "Manage company employees.",
        ["AddEmployee"] = "+ Add employee",
        ["EmployeesList"] = "Employees list",
        ["RegisteredEmployees"] = "employee(s)",
        ["Employee"] = "Employee",
        ["Position"] = "Position",
        ["Salary"] = "Salary",
        ["NoEmployees"] = "No employees",
        ["NoEmployeesText"] = "No employee has been created yet.",
        ["AddFirstEmployee"] = "Create first employee"
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Employees"] = "Anställda",
        ["EmployeesDescription"] = "Hantera företagets anställda.",
        ["AddEmployee"] = "+ Lägg till anställd",
        ["EmployeesList"] = "Anställningslista",
        ["RegisteredEmployees"] = "anställd(a)",
        ["Employee"] = "Anställd",
        ["Position"] = "Position",
        ["Salary"] = "Lön",
        ["NoEmployees"] = "Inga anställda",
        ["NoEmployeesText"] = "Ingen anställd har skapats ännu.",
        ["AddFirstEmployee"] = "Skapa första anställda"
    };
}
