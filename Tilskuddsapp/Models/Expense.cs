namespace Tilskuddsapp.Models
{
    public class Expense
    {
        public int ExpenseId { get; set; }

        public int GrantCaseId { get; set; }
        public GrantCase GrantCase { get; set; } = null!;

        public string ExpenseType { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Amount { get; set; }
    }
}
