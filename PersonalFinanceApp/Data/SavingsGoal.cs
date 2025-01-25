namespace PersonalFinanceApp.Data
{
    public class SavingsGoal
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public decimal GoalAmount { get; set; }
        public decimal CurrentSavings { get; set; }
    }
}
