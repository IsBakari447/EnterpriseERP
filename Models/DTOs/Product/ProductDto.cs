namespace EnterpriseERP.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}