using static Tilskuddsapp.Models.Enums;

namespace Tilskuddsapp.Models
{
    public class GrantCase
    {
        public int GrantCaseId { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public Doctor Doctor { get; set; } = null!;

        public int? SupervisorId { get; set; }
        public Supervisor? Supervisor { get; set; }

        public int Year { get; set; }

        public decimal ApprovedGrant { get; set; }

        public decimal? MaximumGrant { get; set; }

        public decimal SupervisionHours { get; set; }

        public GrantCaseStatus GrantCaseStatus { get; set; }

        public string? Notes { get; set; }

        public ICollection<Expense> Expenses { get; set; }
            = new List<Expense>();
    }
}
