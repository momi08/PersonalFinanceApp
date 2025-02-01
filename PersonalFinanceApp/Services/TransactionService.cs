using Microsoft.EntityFrameworkCore;
using PersonalFinanceApp.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class TransactionService
{
    private readonly AppDbContext _dbContext;

    public TransactionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public List<string> ErrorMessages { get; set; } = new List<string>();

    public async Task<(bool success, decimal newBalance, List<string> errorMessages, List<Transaction> updatedTransactions)>
    HandleManualBalanceSubmitAsync(string userName, decimal amount, List<Transaction> transactions)
    {
        List<string> errorMessages = new List<string>();

        if (amount <= 0)
        {
            errorMessages.Clear();
            errorMessages.Add("Amount cannot be zero or negative.");
            return (false, 0, errorMessages, transactions);
        }
        var manualTransaction = new Transaction
        {
            UserName = userName,
            Amount = amount,
            Category = "Manual Balance",
            Type = "Income",
            Date = DateTime.Now,
            Description = "Manual Balance Addition",
            IsManuallyAdded = true
        };
        _dbContext.Transactions.Add(manualTransaction);
        await _dbContext.SaveChangesAsync();
        var updatedTransactions = await _dbContext.Transactions
            .Where(t => t.UserName == userName)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
        decimal balance = updatedTransactions
            .Where(t => t.UserName == userName)
            .Sum(t => t.Type == "Income" ? t.Amount : -t.Amount);

        decimal newBalance = balance;
        return (true, newBalance, errorMessages, updatedTransactions);
    }

    public async Task<decimal> RecalculateBalanceAsync(string userName)
    {
        var transactions = await _dbContext.Transactions
            .Where(t => t.UserName == userName)
            .ToListAsync();
        decimal balance = transactions
            .Sum(t => t.Type == "Income" ? t.Amount : -t.Amount);

        return balance;
    }
    public async Task<(bool success, List<string> errorMessages)> HandleEditTransactionAsync(
    Transaction transactionToEdit,
    List<Transaction> transactions,
    decimal balance,
    List<string> errorMessages)
    {
        errorMessages.Clear();
        if (transactionToEdit.Amount <= 0)
        {
            errorMessages.Add("The amount cannot be zero or negative.");
            return (false, errorMessages);
        }

        if (string.IsNullOrEmpty(transactionToEdit.Description))
        {
            errorMessages.Add("You must enter the description.");
        }
        else if (!Regex.IsMatch(transactionToEdit.Description, @"^[A-Za-z\s]+$"))
        {
            errorMessages.Add("The description must contain only letters and spaces.");
        }

        if (string.IsNullOrEmpty(transactionToEdit.Category) || transactionToEdit.Category == "Select a Category")
        {
            errorMessages.Add("Please select a category for the transaction.");
        }
        decimal balanceBeforeTransaction = balance;
        var originalTransaction = transactions.FirstOrDefault(t => t.Id == transactionToEdit.Id);

        if (originalTransaction != null)
        {
            if (originalTransaction.Type == "Income")
            {
                balanceBeforeTransaction -= originalTransaction.Amount;
            }
            else if (originalTransaction.Type == "Expense")
            {
                balanceBeforeTransaction += originalTransaction.Amount;
            }
        }
        if (transactionToEdit.Type == "Expense" && balanceBeforeTransaction < transactionToEdit.Amount)
        {
            errorMessages.Add("Insufficient balance for this expense.");
            return (false, errorMessages);
        }

        if (errorMessages.Any())
        {
            return (false, errorMessages);
        }

        try
        {
            if (originalTransaction != null)
            {
                if (originalTransaction.Type == "Expense")
                {
                    balance -= originalTransaction.Amount;
                }
                else if (originalTransaction.Type == "Income")
                {
                    balance += originalTransaction.Amount;
                }
                if (transactionToEdit.Type == "Expense")
                {
                    balance -= transactionToEdit.Amount;
                }
                else if (transactionToEdit.Type == "Income")
                {
                    balance += transactionToEdit.Amount;
                }

                originalTransaction.Description = transactionToEdit.Description;
                originalTransaction.Amount = transactionToEdit.Amount;
                originalTransaction.Category = transactionToEdit.Category;
                originalTransaction.Type = transactionToEdit.Type;
                originalTransaction.Date = transactionToEdit.Date;

                _dbContext.Transactions.Update(originalTransaction);
                await _dbContext.SaveChangesAsync();
                return (true, errorMessages);
            }

            return (false, errorMessages);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return (false, errorMessages);
        }
    }
    public async Task<(bool isSuccess, List<string> errorMessages, List<Transaction> updatedFilteredTransactions)> HandleValidSubmitAsync(
        string userName,
        decimal balance,
        Transaction newTransaction,
        List<Transaction> transactions)
    {
        var errorMessages = new List<string>();
        bool isDatabaseOperationInProgress = false;

        errorMessages.Clear();

        if (string.IsNullOrEmpty(newTransaction.Description))
        {
            errorMessages.Add("You must enter the description.");
        }
        else if (!Regex.IsMatch(newTransaction.Description, @"^[A-Za-z\s]+$"))
        {
            errorMessages.Add("The description must contain only letters and spaces.");
        }

        isDatabaseOperationInProgress = true;

        if (string.IsNullOrEmpty(newTransaction.Category) || newTransaction.Category == "Select a Category")
        {
            errorMessages.Add("Please select a category for the transaction.");
        }

        if (newTransaction.Amount <= 0)
        {
            errorMessages.Add("The amount cannot be zero or negative.");
        }

        if (newTransaction.Type == "Expense" && balance < newTransaction.Amount)
        {
            errorMessages.Add("Insufficient balance for this expense.");
        }

        if (errorMessages.Any())
        {
            return (false, errorMessages, transactions);
        }

        if (string.IsNullOrEmpty(newTransaction.Type))
        {
            newTransaction.Type = "Income";
        }

        try
        {
            newTransaction.UserName = userName;

            if (newTransaction.Type == "Income")
            {
                balance += newTransaction.Amount;
            }
            else if (newTransaction.Type == "Expense")
            {
                balance -= newTransaction.Amount;
            }

            newTransaction.Date = DateTime.Now;

            _dbContext.Transactions.Add(newTransaction);
            await _dbContext.SaveChangesAsync();

            transactions.Add(newTransaction);
            var updatedFilteredTransactions = new List<Transaction>(transactions);

            var updatedBalance = await RecalculateBalanceAsync(userName);
            balance = updatedBalance;

            return (true, errorMessages, updatedFilteredTransactions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            errorMessages.Add("An error occurred while saving the transaction.");
            return (false, errorMessages, transactions);
        }
        finally
        {
            isDatabaseOperationInProgress = false;
        }
    }

    public async Task ResetBalanceAsync(string userName, List<Transaction> transactions, List<Transaction> filteredTransactions)
    {
        try
        {
            var userTransactions = await _dbContext.Transactions
                .Where(t => t.UserName == userName)
                .ToListAsync();
            _dbContext.Transactions.RemoveRange(userTransactions);
            await _dbContext.SaveChangesAsync();
            transactions.Clear();
            filteredTransactions.Clear();

            decimal balance = 0;

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error resetting balance: {ex.Message}");
            throw;
        }
    }
    public async Task<(bool success, decimal newBalance)> DeleteTransactionAsync(Transaction transactionToDelete, string userName, List<Transaction> transactions, List<Transaction> filteredTransactions)
    {
        if (transactionToDelete == null)
        {
            return (false, 0);
        }

        try
        {
            _dbContext.Transactions.Remove(transactionToDelete);
            await _dbContext.SaveChangesAsync();
            transactions.Remove(transactionToDelete);

            decimal newBalance = transactions
                .Where(t => t.UserName == userName)
                .Sum(t => t.Type == "Income" ? t.Amount : -t.Amount);
            filteredTransactions = new List<Transaction>(transactions);

            return (true, newBalance);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting transaction: {ex.Message}");
            return (false, 0);
        }
    }
}
