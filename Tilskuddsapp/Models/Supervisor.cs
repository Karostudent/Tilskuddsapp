using static Tilskuddsapp.Models.Enums;


namespace Tilskuddsapp.Models;

public class Supervisor
{
    public int SupervisorId { get; set; }

    public string Name { get; set; } = string.Empty;

    public SupervisorType SupervisorType { get; set; }

    public decimal HourlyRate { get; set; }

    public bool Active { get; set; } = true;

    public ICollection<GrantCase> GrantCases { get; set; }
            = new List<GrantCase>();
}
