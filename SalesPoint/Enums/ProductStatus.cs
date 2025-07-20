namespace SalesPoint.Enums
{
    public enum ProductStatus
    {
        Active = 1,         // Available for sale
        Inactive = 2,       // Not for sale but not deleted
        Discontinued = 3,   // Permanently removed by supplier/brand
        OutOfStock = 4      // Temporarily unavailable
    }
}
