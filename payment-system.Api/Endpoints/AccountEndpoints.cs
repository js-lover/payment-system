using payment_system.Api.DTOs;
using payment_system.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using payment_system.Domain.Entities;
using Microsoft.Identity.Client;

namespace payment_system.Api.Endpoints
{
    public static class AccountEndpoints
    {
        public static void MapAccountEndpoints(this WebApplication app)
        {
            app.MapGet("/api/accounts/{accountId}/transactions", async (Guid accountId, AppDbContext db) =>
            {
                var account = await db.Accounts.Include(a => a.Customer).FirstOrDefaultAsync(a => a.Id == accountId);

                if (account == null)
                {
                    return Results.NotFound("The account not found");
                }

                

                var transactions = await db.Transactions
                        .Include(t => t.ChildTransactions)
                        .Where(t => t.AccountId == accountId && t.ReferenceTransactionId == null)
                        .OrderByDescending(t => t.TransactionDate)
                        .ToListAsync();

                var transactionDetails = transactions.Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Currency = t.Currency.ToString(),
                    TransactionStatus = t.Status.ToString(),
                    Date = t.TransactionDate,
                    TransactionType = t.TransactionType.ToString(),
                    Description = t.Description,
                    Children = t.ChildTransactions.Select(c => new TransactionDto
                    {
                        Id = c.Id,
                        Amount = c.Amount,
                        Currency = c.Currency.ToString(),
                        TransactionStatus = c.Status.ToString(),
                        Date = c.TransactionDate,
                        TransactionType = c.TransactionType.ToString(),
                        Description = c.Description
                    }).ToList()
                }).ToList();


                var result = new AccountDetailsDto
                {
                    Id = account.Id,
                    Name = account.Customer.Name,
                    Balance = account.Balance,
                    Currency = account.Currency.ToString(),
                    Transactions = transactionDetails
                };

                return Results.Ok(result);
            });
            
        }
    }
}