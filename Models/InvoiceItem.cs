using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class InvoiceItem
    {
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        public int? ProductId { get; set; }

        [Required]
        [Display(Name = "Produit / Service")]
        public string ProductName { get; set; } = "";

        [Display(Name = "Description")]
        public string Description { get; set; } = "";

        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [Display(Name = "Quantité")]
        public decimal Quantity { get; set; } = 1;

        [Display(Name = "Prix unitaire")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Remise (%)")]
        public decimal Discount { get; set; }

        [Display(Name = "TVA (%)")]
        public decimal VatRate { get; set; } = 20;

        [Display(Name = "TVA incluse")]
        public bool VatIncluded { get; set; } = true;

        [Display(Name = "Total HT")]
        public decimal TotalHT { get; set; }

        [Display(Name = "Montant TVA")]
        public decimal VatAmount { get; set; }

        [Display(Name = "Total TTC")]
        public decimal TotalTTC { get; set; }

        public Invoice? Invoice { get; set; }

        public Product? Product { get; set; }
    }
}