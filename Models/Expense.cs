using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "La catégorie est obligatoire")]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le montant est obligatoire")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Le montant doit être supérieur à 0")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "La date est obligatoire")]
        public DateTime ExpenseDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Description { get; set; }

        public string? PaymentMethod { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}