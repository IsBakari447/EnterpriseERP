using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string Category { get; set; } = "";

        public decimal PurchasePrice { get; set; }

        public decimal SalePrice { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}