using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class Permission
    {
        public int Id { get; set; }

        [Required]
        public string Module { get; set; } = "";

        [Required]
        public string Action { get; set; } = "";

        public string Description { get; set; } = "";
    }
}