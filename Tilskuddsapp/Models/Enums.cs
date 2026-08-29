namespace Tilskuddsapp.Models
{
    public class Enums
    {
        public enum DoctorType
        {
            Legevakt,
            SelvstendigNæringsdrivende
        }

        public enum SupervisorType
        {
            Intern,
            Ekstern
        }

        public enum GrantCaseStatus
        {
            UnderBehandling,
            Godkjent,
            Avvist
        }
    }
}