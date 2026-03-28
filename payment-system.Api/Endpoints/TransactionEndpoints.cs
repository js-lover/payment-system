using payment_system.Application.DTOs.Account;
using payment_system.Application.DTOs.Transaction;
using payment_system.Application.Services.Interfaces;

namespace payment_system.Api.Endpoints
{
    /// <summary>
    /// Transaction endpoint'lerini tanımlar
    /// Minimal API kullanarak HTTP request'lerini handle eder
    /// </summary>
    public static class TransactionEndpoints
    {
        public static void MapTransactionEndpoints(this WebApplication app)
        {
            // Endpoint grubu oluştur - tüm transaction endpoint'leri /api/transactions altında
            var group = app.MapGroup("/api/transactions")
                .WithName("Transactions")
                .WithOpenApi();

            // GET /api/transactions - Tüm transaction'ları getir
            group.MapGet("/", GetAllTransactions)
                .WithName("GetAllTransactions")
                .WithOpenApi()
                .WithDescription("Tüm transaction'ları child transaction'ları ile beraber getirir");

            // POST /api/transactions - Yeni transaction oluştur
            group.MapPost("/", CreateTransaction)
                .WithName("CreateTransaction")
                .WithOpenApi()
                .WithDescription("Yeni transaction oluşturur ve validasyonlar yapılır");
        }

        /// <summary>
        /// GET /api/transactions handler
        /// </summary>
        /// <param name="transactionService">DI container'dan injekte edilen service</param>
        /// <returns>Transaction listesi veya hata</returns>
        private static async Task<IResult> GetAllTransactions(
            ITransactionService transactionService)
        {
            // Service'i çağır
            var result = await transactionService.GetAllTransactionsAsync();

            // Sonuca göre response döndür
            if (result.IsSuccess)
            {
                return Results.Ok(result.Data);
            }

            return Results.BadRequest(new { message = result.Message });
        }

        /// <summary>
        /// POST /api/transactions handler
        /// </summary>
        /// <param name="request">Request body'den otomatik map edilir</param>
        /// <param name="transactionService">DI container'dan injekte edilen service</param>
        /// <returns>Oluşturulan transaction veya hata</returns>
        private static async Task<IResult> CreateTransaction(
            CreateTransactionRequest request, // JSON'dan otomatik map edilir
            ITransactionService transactionService // DI container'dan injekte edilen service
            )
        {
            // Service'i çağır
            var result = await transactionService.CreateTransactionAsync(request);

            // Sonuca göre response döndür
            if (result.IsSuccess)
            {
                return Results.Created(
                    $"/api/transactions/{result.Data?.Id}", 
                    result.Data);
            }

            // Hata koduna göre tepki ver
            return result.StatusCode switch
            {
                404 => Results.NotFound(new { message = result.Message }),
                400 => Results.BadRequest(new { message = result.Message }),
                _ => Results.StatusCode(result.StatusCode ?? 500)
            };
        }
    }
}