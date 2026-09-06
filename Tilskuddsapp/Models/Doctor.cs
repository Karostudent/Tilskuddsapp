namespace Tilskuddsapp.Models;

public class Doctor
{
    public int Id { get; set; }
    // Preserve the identifier as text, including any leading zeroes.
    public string HprNumber { get; set; } = "";
    public string Name { get; set; } = "";
    public List<string> Professions { get; set; } = [];
}
