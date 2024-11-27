using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class MenuItemNotAvailableException : CampusEatsException
    {
        public int ItemId { get; }
        public string ItemName { get; }

        public MenuItemNotAvailableException(int itemId, string itemName)
            : base($"Menu item '{itemName}' (ID: {itemId}) is currently not available.")
        {
            ItemId = itemId;
            ItemName = itemName;
        }
    }
}
