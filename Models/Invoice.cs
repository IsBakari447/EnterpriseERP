using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        public DateTime InvoiceDate { get; set; } = DateTime.Now;

        public string InvoiceNumber { get; set; } = "";

        public string Status { get; set; } = "Unpaid";

        public bool VatIncluded { get; set; } = true;

        public decimal VatRate { get; set; } = 20;

        public decimal SubTotal { get; set; }

        public decimal VatAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = "Cash";

        public string ThankYouMessage { get; set; } =
            "Merci beaucoup pour votre confiance. Nous serons ravis de vous accueillir à nouveau la prochaine fois.";

        public Client? Client { get; set; }

        public List<InvoiceItem> Items { get; set; } = new();
    }
}