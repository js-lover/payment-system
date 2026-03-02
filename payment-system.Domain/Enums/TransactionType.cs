namespace payment_system.Domain.Enums
{
    public enum TransactionType
    {
        None = 0,
        Sale = 1,
        Refund = 2,
        Void = 3,
        Deposit = 4,
        Withdrawal = 5
    }
}