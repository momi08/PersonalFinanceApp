﻿namespace PersonalFinanceApp.Data
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
    }
}
