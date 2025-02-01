using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Data;


public class UserService
{
    private readonly AppDbContext _dbContext;

    public UserService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<(List<Transaction> transactions, decimal balance)> LoadUserDataAsync(string userName)
    {
        var transactions = await _dbContext.Transactions
            .Where(t => t.UserName == userName)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
        var balance = transactions
            .Where(t => t.UserName == userName)
            .Sum(t => t.Type == "Income" ? t.Amount : -t.Amount);
        return (transactions, balance);
    }
}
