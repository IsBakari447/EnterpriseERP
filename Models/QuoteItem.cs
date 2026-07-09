using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class QuoteItem
    {
        public int Id { get; set; }

        public int QuoteId { get; set; }

        public Quote? Quote { get; set; }

        [Required(ErrorMessage = "Le produit est obligatoire")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La quantité doit être supérieure à 0")]
        public int Quantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0")]
        public decimal UnitPrice { get; set; }

        public decimal DiscountRate { get; set; } = 0;

        public decimal DiscountAmount { get; set; }

        public decimal TaxRate { get; set; } = 0;

        public decimal TaxAmount { get; set; }

        public decimal LineTotal { get; set; }
    }
}