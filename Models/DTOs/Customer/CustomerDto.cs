namespace EnterpriseERP.DTOs.Customer
{
    public class CustomerDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}