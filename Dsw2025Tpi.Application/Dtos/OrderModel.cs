using System;
using System.Collections.Generic;

namespace Dsw2025Tpi.Application.Dtos
{
    public class OrderModel
    {
        public record OrderItemRequest(
            Guid ProductId,
            int Quantity,
            string Name,
            string Description,
            decimal CurrentUnitPrice
        );

        public record Request(
            Guid CustomerId,
            string ShippingAddress,
            string BillingAddress,
            List<OrderItemRequest> OrderItems
        );

        public record OrderItemResponse(
            Guid ProductId,
            string Name,
            string Description,
            decimal UnitPrice,
            int Quantity,
            decimal Subtotal
        );

        public record Response(
            Guid Id,
            Guid CustomerId,
            string ShippingAddress,
            string BillingAddress,
            DateTime Date,
            decimal TotalAmount,
            List<OrderItemResponse> OrderItems,
            string Status
        );
    }
}