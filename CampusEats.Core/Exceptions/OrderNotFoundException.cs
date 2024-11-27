using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class OrderNotFoundException : CampusEatsException
    {
        public int OrderId { get; }
        public OrderNotFoundException(int orderId)
            : base($"Order with ID {orderId} was not found.")
        {
            OrderId = orderId;
        }
    }
}
