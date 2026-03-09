using payment_system.Api.DTOs;
using payment_system.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

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
        }
    }
}