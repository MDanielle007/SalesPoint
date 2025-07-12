namespace SalesPoint.Enum
{
    public enum TransactionStatus
    {
        Pending = 1,     // Waiting for payment (e.g., reserved)
        Completed = 2,   // Successfully paid and recorded
        Cancelled = 3,   // Voided before completion
        Refunded = 4     // Reversed after being completed
    }
}
