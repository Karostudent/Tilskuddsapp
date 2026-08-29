using static Tilskuddsapp.Models.Enums;


namespace Tilskuddsapp.Models;

public class Supervisor
{
    public int SupervisoId { get; set; }

    public string Name { get; set; } = string.Empty;

    public SupervisorType SupervisorType { get; set; }

    public decimal HourlyRate { get; set; }

    public bool Active { get; set; } = true;
}
