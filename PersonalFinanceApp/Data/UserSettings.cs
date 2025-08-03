namespace PersonalFinanceApp.Data
{
    public class UserSettings
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; } 

        public bool EmailNotifications { get; set; } = true;

        public string Theme { get; set; } = "light";

        public string Currency { get; set; } = "EUR";
    }
}
