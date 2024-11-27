using CampusEats.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class InvalidOrderStatusTransitionException : CampusEatsException
    {
        public OrderStatus CurrentStatus { get; }
        public OrderStatus NewStatus { get; }

        public InvalidOrderStatusTransitionException(OrderStatus currentStatus, OrderStatus newStatus)
            : base($"Cannot transition order from status '{currentStatus}' to '{newStatus}'.")
        {
            CurrentStatus = currentStatus;
            NewStatus = newStatus;
        }
    }
}
