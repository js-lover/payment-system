namespace payment_system.Domain.Enums
{
    public enum Currency
    {
        //the reason behind why we use enums for currency is to have a fixed set of values
        //and why we use none is to defend against default values. 
        //because we do not want any accidental usage of default values 
        None = 0,
        USD = 1,
        EUR = 2,
        GBP = 3,
        TRY = 4,
        JPY = 5,
        CAD = 6,
        AUD = 7,
        CNY = 8
    }
}