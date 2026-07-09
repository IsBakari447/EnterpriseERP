namespace EnterpriseERP.Services.Translations;

public static class Products
{
    public static readonly Dictionary<string,string> Fr = new()
    {
        ["Products"] = "Produits",
        ["ProductsDescription"] = "Gestion du catalogue, des prix et du stock.",
        ["AddProduct"] = "+ Ajouter un produit",
        ["ProductsList"] = "Liste des produits",
        ["RegisteredProducts"] = "produit(s)",
        ["Category"] = "Catégorie",
        ["PurchasePrice"] = "Prix achat",
        ["SalePrice"] = "Prix vente",
        ["Quantity"] = "Quantité",
        ["NoProducts"] = "Aucun produit",
        ["NoProductsText"] = "Aucun produit n'a encore été enregistré.",
        ["AddFirstProduct"] = "Créer le premier produit"
    };

    public static readonly Dictionary<string,string> En = new()
    {
        ["Products"] = "Products",
        ["ProductsDescription"] = "Manage catalog, prices and inventory.",
        ["AddProduct"] = "+ Add product",
        ["ProductsList"] = "Products list",
        ["RegisteredProducts"] = "product(s)",
        ["Category"] = "Category",
        ["PurchasePrice"] = "Purchase price",
        ["SalePrice"] = "Sale price",
        ["Quantity"] = "Quantity",
        ["NoProducts"] = "No products",
        ["NoProductsText"] = "No product has been created yet.",
        ["AddFirstProduct"] = "Create first product"
    };

    public static readonly Dictionary<string,string> Sv = new()
    {
        ["Products"] = "Produkter",
        ["ProductsDescription"] = "Hantera katalog, priser och lager.",
        ["AddProduct"] = "+ Lägg till produkt",
        ["ProductsList"] = "Produktlista",
        ["RegisteredProducts"] = "produkt(er)",
        ["Category"] = "Kategori",
        ["PurchasePrice"] = "Inköpspris",
        ["SalePrice"] = "Försäljningspris",
        ["Quantity"] = "Antal",
        ["NoProducts"] = "Inga produkter",
        ["NoProductsText"] = "Ingen produkt har skapats ännu.",
        ["AddFirstProduct"] = "Skapa första produkten"
    };
}
