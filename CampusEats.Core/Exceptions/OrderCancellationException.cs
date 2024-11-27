using CampusEats.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class OrderCancellationException : CampusEatsException
    {
        public int OrderId { get; }
        public OrderStatus CurrentStatus { get; }

        public OrderCancellationException(int orderId, OrderStatus currentStatus)
            : base($"Cannot cancel order {orderId}. Current status: {currentStatus}")
        {
            OrderId = orderId;
            CurrentStatus = currentStatus;
        }
    }
}
