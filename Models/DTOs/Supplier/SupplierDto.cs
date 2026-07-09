namespace EnterpriseERP.DTOs.Supplier
{
    public class SupplierDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string ContactPerson { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}