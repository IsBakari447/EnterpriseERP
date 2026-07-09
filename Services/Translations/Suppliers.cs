namespace EnterpriseERP.Services.Translations;

public static class Suppliers
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Suppliers"] = "Fournisseurs",
        ["SuppliersDescription"] = "Gestion des fournisseurs et partenaires.",
        ["AddSupplier"] = "+ Ajouter un fournisseur",
        ["SuppliersList"] = "Liste des fournisseurs",
        ["RegisteredSuppliers"] = "fournisseur(s)",
        ["Contact"] = "Contact",
        ["NoSuppliers"] = "Aucun fournisseur",
        ["NoSuppliersText"] = "Aucun fournisseur n'a encore été enregistré.",
        ["AddFirstSupplier"] = "Créer le premier fournisseur"
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Suppliers"] = "Suppliers",
        ["SuppliersDescription"] = "Manage suppliers and business partners.",
        ["AddSupplier"] = "+ Add supplier",
        ["SuppliersList"] = "Suppliers list",
        ["RegisteredSuppliers"] = "supplier(s)",
        ["Contact"] = "Contact",
        ["NoSuppliers"] = "No suppliers",
        ["NoSuppliersText"] = "No supplier has been created yet.",
        ["AddFirstSupplier"] = "Create first supplier"
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Suppliers"] = "Leverantörer",
        ["SuppliersDescription"] = "Hantera leverantörer och affärspartners.",
        ["AddSupplier"] = "+ Lägg till leverantör",
        ["SuppliersList"] = "Leverantörslista",
        ["RegisteredSuppliers"] = "leverantör(er)",
        ["Contact"] = "Kontakt",
        ["NoSuppliers"] = "Inga leverantörer",
        ["NoSuppliersText"] = "Ingen leverantör har skapats ännu.",
        ["AddFirstSupplier"] = "Skapa första leverantören"
    };
}
