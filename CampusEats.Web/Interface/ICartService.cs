using CampusEats.Web.Models.Order;

namespace CampusEats.Web.Interface
{
    public interface ICartService
    {
        CartViewModel GetCart();
        void AddItem(int restaurantId, string restaurantName, int menuItemId, string name, decimal price, int quantity = 1, string instructions = "");
        void UpdateItemQuantity(int menuItemId, int quantity);
        void RemoveItem(int menuItemId);
        void ClearCart();
        bool HasItems();
    }
}
