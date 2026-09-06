namespace Tilskuddsapp.Models;

public class Reimbursement
{
    public int Id { get; set; }
    public int GrantApplicationId { get; set; }
    public decimal? AbsenceCompensation { get; set; }
    public decimal? LearningActivityExpenses { get; set; }
    public decimal? SupervisionExpenses { get; set; }
    // null means unanswered; false is an explicit "Nei".
    public bool? HasAdditionalSupervisionCosts { get; set; }
    public decimal? AdditionalSupervisionCosts { get; set; }
}
