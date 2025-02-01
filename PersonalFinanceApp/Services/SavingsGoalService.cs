using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Data;

public class SavingsGoalService
{
    private readonly AppDbContext _dbContext;

    public SavingsGoalService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SavingsGoal> LoadSavingsGoalAsync(string userName)
    {
        var existingGoal = await _dbContext.SavingsGoals
            .FirstOrDefaultAsync(s => s.UserName == userName);

        if (existingGoal != null)
        {
            return existingGoal;
        }
        return new SavingsGoal();
    }

    public async Task<bool> UpdateSavingsGoalAsync(string userName, decimal goalAmount, decimal currentSavings, List<string> errorMessages)
    {
        if (goalAmount <= 0 || string.IsNullOrEmpty(userName))
        {
            errorMessages.Add("Please enter a valid savings goal amount.");
            return false;
        }
        var existingGoal = await _dbContext.SavingsGoals
            .FirstOrDefaultAsync(s => s.UserName == userName);

        if (existingGoal != null)
        {
            existingGoal.GoalAmount = goalAmount;
            existingGoal.CurrentSavings = currentSavings;
            _dbContext.SavingsGoals.Update(existingGoal);
        }
        else
        {
            var newGoal = new SavingsGoal
            {
                UserName = userName,
                GoalAmount = goalAmount,
                CurrentSavings = currentSavings
            };
            _dbContext.SavingsGoals.Add(newGoal);
        }
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
