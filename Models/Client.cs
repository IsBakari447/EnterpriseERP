using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        public string CompanyName { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Address { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}