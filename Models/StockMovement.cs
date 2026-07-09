namespace EnterpriseERP.Models
{
    public class StockMovement
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public string Type { get; set; } = ""; 
        // IN ou OUT

        public int Quantity { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public Product? Product { get; set; }
    }
}