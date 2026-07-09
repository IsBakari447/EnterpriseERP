using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        public int InvoiceId { get; set; }

        public decimal Amount { get; set; }

        public string Method { get; set; } = "Cash";

        public string Reference { get; set; } = "";

        public DateTime PaymentDate { get; set; } = DateTime.Now;

        public Invoice? Invoice { get; set; }
    }
}