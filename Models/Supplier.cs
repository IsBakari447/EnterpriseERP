using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = "";

        public string ContactPerson { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Address { get; set; } = "";

        public string Category { get; set; } = "";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}