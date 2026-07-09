using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public Client? Client { get; set; }

        public Product? Product { get; set; }
    }
}