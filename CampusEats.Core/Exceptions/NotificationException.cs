using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class NotificationException : CampusEatsException
    {
        public int? NotificationId { get; }

        public NotificationException(string message, int? notificationId = null)
            : base(message)
        {
            NotificationId = notificationId;
        }
    }
}
