using payment_system.Application.DTOs.Account;
using payment_system.Application.DTOs.Transaction;
using payment_system.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using payment_system.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace payment_system.Api.Endpoints
{
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this WebApplication app)
        {
            app.MapGet("/api/transactions", async (AppDbContext db) =>
            {
                var transactions = await db.Transactions.Include(t => t.ChildTransactions).Where(t => t.ReferenceTransactionId == null).ToListAsync();

                var result = transactions.Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    TransactionStatus = t.Status.ToString(),
                    TransactionType = t.TransactionType.ToString(),
                    Date = t.TransactionDate,
                    Description = t.Description,
                    Currency = t.Currency.ToString(),


                    Children = t.ChildTransactions.Select(c => new TransactionDto
                    {

                        Id = c.Id,
                        Amount = c.Amount,
                        TransactionStatus = c.Status.ToString(),
                        TransactionType = c.TransactionType.ToString(),
                        Date = c.TransactionDate,
                        Description = c.Description,
                        Currency = c.Currency.ToString(),
                    }).ToList()
                }).ToList();

                return Results.Ok(result);

            });


            app.MapPost("/api/transactions", async (CreateTransactionRequest request, AppDbContext db) =>
            {
                //------------------------------------------------------------------------------------------
                //TRANSACTION VALIDATIONS
                //THIS IS WHERE WE VALIDATE THE TRANSACTION REQUEST BEFORE WE CREATE THE TRANSACTION OBJECT
                //IT ENSURES THAT ALL REQUIRED FIELDS ARE PRESENT AND VALID
                //AND HELPS TO PREVENT ERRORS LATER IN THE PROCESS
                //------------------------------------------------------------------------------------------

                //transaction amount validation
                if (request.Amount <= 0)
                {
                    return Results.BadRequest("Amount must be greater than zero.");
                }

                //account validation
                var account = await db.Accounts.FindAsync(request.AccountId);
                if (account == null)
                {
                    return Results.NotFound("Account not found.");
                }

                Transaction? referenceTransaction = null;

                //transaction type validation -> if transaction is a refund and no reference transaction id is provided
                // refund transactions validations
                if (request.TransactionType == Domain.Enums.TransactionType.Refund)
                {
                    if (!request.ReferenceTransactionId.HasValue)
                    {
                        return Results.BadRequest("Refund transactions must reference an existing transaction.");
                    }


                    referenceTransaction = await db.Transactions
                        .Include(t => t.ChildTransactions)
                        .FirstOrDefaultAsync(t => t.Id == request.ReferenceTransactionId);

                    if (referenceTransaction == null)
                    {
                        return Results.BadRequest("Invalid reference transaction.");
                    }

                    // Check if the reference transaction belongs to the same account
                    if (referenceTransaction.AccountId != request.AccountId)
                    {
                        return Results.BadRequest("You cannot refund a transaction that doesn't belong to this account.");
                    }

                    // Check if the reference transaction is a sale
                    if (referenceTransaction.TransactionType != Domain.Enums.TransactionType.Sale)
                    {
                        return Results.BadRequest("Only sales transactions can be refunded.");
                    }

                    // Calculate the total refund amount
                    // total refund amount is the sum of all child transaction amounts
                    // cannot exceed the original transaction amount
                    var totalRefundAmount = referenceTransaction.ChildTransactions.Sum(x => x.Amount);
                    if (totalRefundAmount + request.Amount > referenceTransaction.Amount)
                    {
                        return Results.BadRequest("Refund amount cannot be greater than the original transaction amount.");
                    }
                    account.Balance += request.Amount;
                }

                // SALE TRANSACTION VALIDATION
                if (request.TransactionType == Domain.Enums.TransactionType.Sale)
                {
                    if (account.Balance < request.Amount)
                    {
                        return Results.BadRequest("Don't have enough money to complete the sale");
                    }

                    // Deduct the transaction amount from the account balance
                    account.Balance -= request.Amount;

                }

                // CREATING THE TRANSACTION
                // the reason why we create transaction at the bottom is to ensure all validations are passed
                // if we created the transaction at the beginning and any validation failed later,
                // we would have to roll back the transaction, which is more complex
                var transaction = new Transaction
                {
                    AccountId = request.AccountId,
                    Id = Guid.NewGuid(),
                    Amount = request.Amount,
                    Description = request.Description,
                    Currency = request.Currency,
                    TransactionType = request.TransactionType,
                    ReferenceTransactionId = request.ReferenceTransactionId,
                    TransactionDate = DateTime.UtcNow,
                    Status = Domain.Enums.TransactionStatus.Success
                };

                // UPDATING THE ACCOUNT IN THE DATABASE
                //EF CORE REALISEZ THE CHANGES AND UPDATES
                db.Transactions.Add(transaction);
                await db.SaveChangesAsync();

                //transaction created successfully
                return Results.Created($"/api/transactions/{transaction.Id}", new TransactionDto
                {
                    Id = transaction.Id,
                    Amount = transaction.Amount,
                    TransactionStatus = transaction.Status.ToString(),
                    TransactionType = transaction.TransactionType.ToString(),
                    Date = transaction.TransactionDate,
                    Description = transaction.Description,
                    Currency = transaction.Currency.ToString(),
                    Children = new List<TransactionDto>()
                });
            });
        }
    }
}