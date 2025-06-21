using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dsw2025Tpi.Domain.Entities
{
    public class OrderItem : EntityBase
    {
        public OrderItem() 
        {
            
        }
        public OrderItem(int subtotal, decimal price, decimal quantity)
        {
            Subtotal = subtotal;
            UnitPrice = price;
            Quantity = quantity;
        }

        public int Subtotal { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
    }
}
