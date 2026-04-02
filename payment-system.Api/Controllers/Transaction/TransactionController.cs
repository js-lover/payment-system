using System.Transactions;
using Microsoft.AspNetCore.Mvc;
using payment_system.Application.Services.Interfaces;

namespace payment_system.Api.Controllers
{
    


[ApiController]
[Route("api/[controller]")]
public partial class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService; 

    public TransactionController(ITransactionService transactionService) 
        => _transactionService = transactionService;
}

}