namespace EnterpriseERP.Models
{
    public class Presence
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public DateTime Date { get; set; } = DateTime.Today;

        public TimeSpan? CheckIn { get; set; }

        public TimeSpan? CheckOut { get; set; }

        public Employee? Employee { get; set; }
    }
}