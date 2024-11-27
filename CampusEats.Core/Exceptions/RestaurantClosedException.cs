using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class RestaurantClosedException : CampusEatsException
    {
        public int RestaurantId { get; }
        public TimeSpan OpeningTime { get; }
        public TimeSpan ClosingTime { get; }

        public RestaurantClosedException(int restaurantId, TimeSpan openingTime, TimeSpan closingTime)
            : base($"Restaurant {restaurantId} is currently closed. Operating hours: {openingTime:hh\\:mm} - {closingTime:hh\\:mm}")
        {
            RestaurantId = restaurantId;
            OpeningTime = openingTime;
            ClosingTime = closingTime;
        }
    }
}
