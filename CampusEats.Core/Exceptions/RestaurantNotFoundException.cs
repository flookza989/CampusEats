using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class RestaurantNotFoundException : CampusEatsException
    {
        public int RestaurantId { get; }
        public RestaurantNotFoundException(int restaurantId)
            : base($"Restaurant with ID {restaurantId} was not found.")
        {
            RestaurantId = restaurantId;
        }
    }
}
