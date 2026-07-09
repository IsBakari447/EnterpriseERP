namespace EnterpriseERP.Models
{
    public class RolePermission
    {
        public int Id { get; set; }

        public string Role { get; set; } = "";

        public int PermissionId { get; set; }

        public Permission? Permission { get; set; }
    }
}