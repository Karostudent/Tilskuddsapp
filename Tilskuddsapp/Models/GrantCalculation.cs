namespace Tilskuddsapp.Models
{
    public class GrantCalculation
    {
        public decimal ApprovedGrant { get; set; }

        public decimal? MaximumGrant { get; set; }

        public decimal SupervisionHours { get; set; } = 0;
        public decimal TotalExpenses { get; set; }

        public decimal SupervisorAmount { get; set; }

        public decimal DoctorAmount { get; set; }

        public decimal EmergencyClinicAmount { get; set; }

        public decimal MunicipalityAmount { get; set; }

        public decimal Difference { get; set; }

        public bool IsValid { get; set; }

        public List<string> Warnings { get; set; } = new();

    }
}
