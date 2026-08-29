using static Tilskuddsapp.Models.Enums;

namespace Tilskuddsapp.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }

        public string Name { get; set; } = string.Empty;

        public DoctorType DoctorType { get; set; }

        public bool Active { get; set; } = true;

        public ICollection<GrantCase> GrantCases { get; set; }
            = new List<GrantCase>();
    }
}
