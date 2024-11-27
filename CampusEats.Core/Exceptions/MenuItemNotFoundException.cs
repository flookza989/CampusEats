using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusEats.Core.Exceptions
{
    public class MenuItemNotFoundException : CampusEatsException
    {
        public int ItemId { get; }
        public MenuItemNotFoundException(int itemId)
            : base($"Menu item with ID {itemId} was not found.")
        {
            ItemId = itemId;
        }
    }
}
