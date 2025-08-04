namespace PersonalFinanceApp.Data
{
    public class LoginHistory
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime LoginTime { get; set; }
        public string IPAddress { get; set; }

        public User User { get; set; }
    }
}
