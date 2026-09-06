namespace Tilskuddsapp.Models;

public class EmploymentPeriod
{
    public int Id { get; set; }
    public int GrantApplicationId { get; set; }
    public PositionType? PositionType { get; set; }
    public decimal? PositionPercentage { get; set; }
    public DateOnly? EmploymentStartDate { get; set; }
    public DateOnly? FundingFrom { get; set; }
    public DateOnly? FundingThrough { get; set; }
}
