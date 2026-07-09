using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; } = "";

        public string Position { get; set; } = "";

        public string Email { get; set; } = "";

        public string Phone { get; set; } = "";

        public decimal Salary { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}