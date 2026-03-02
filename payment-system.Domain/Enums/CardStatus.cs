namespace payment_system.Domain.Enums
{
    public enum CardStatus
    {
        None = 0,
        WaitingForApproval = 1,
        Active = 2,
        Blocked = 3,
        Expired = 4
    }
}