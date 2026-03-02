namespace payment_system.Domain.Enums
{
    public enum TransactionStatus
    {
        //started from pending because we need to track the initial state of the transaction
        Pending = 0,
        Success = 1,
        Failed = 2
    }
}