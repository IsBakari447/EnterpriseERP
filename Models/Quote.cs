using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Quote
    {
        public int Id { get; set; }

        [Required]
        public string QuoteNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le client est obligatoire")]
        public int ClientId { get; set; }

        public Client? Client { get; set; }

        [Required]
        public DateTime QuoteDate { get; set; } = DateTime.Now;

        public DateTime ValidUntil { get; set; } = DateTime.Now.AddDays(30);

        public string Status { get; set; } = "Brouillon";

        public decimal SubTotal { get; set; }

        public decimal DiscountRate { get; set; } = 0;

        public decimal DiscountAmount { get; set; }

        public decimal TaxRate { get; set; } = 0;

        public decimal TaxAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string? PaymentTerms { get; set; }

        public string? Notes { get; set; }

        public string? InternalNotes { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public List<QuoteItem> Items { get; set; } = new();
    }
}