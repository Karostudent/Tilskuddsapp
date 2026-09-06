namespace Tilskuddsapp.Models;

public class GrantRound
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateOnly FundingFrom { get; set; }
    public DateOnly FundingThrough { get; set; }
    public string CalculationRuleVersion { get; set; } = "";
}
