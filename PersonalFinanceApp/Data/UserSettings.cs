namespace PersonalFinanceApp.Data
{
    public class UserSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; } 
        public string Currency { get; set; } = "EUR";
    }
}
