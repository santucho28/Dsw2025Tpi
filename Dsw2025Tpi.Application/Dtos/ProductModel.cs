namespace Dsw2025Tpi.Application.Dtos
{
    public class ProductModel
    {
        public record Request(string Sku, string InternalCode, string name, string Description, decimal CurrentUnitPrice, int StockQuantity);
        public record Response(
            object id,
            Guid Id,
            string? Sku,
            string? InternalCode,
            string? Name,
            string? Description,
            decimal CurrentUnitPrice,
            int StockQuantity
        );
    }
}