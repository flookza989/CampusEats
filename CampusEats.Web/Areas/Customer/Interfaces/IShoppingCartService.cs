using CampusEats.Web.Areas.Customer.Models.Carts;

namespace CampusEats.Web.Areas.Customer.Interfaces
{
    public interface IShoppingCartService
    {
        ShoppingCart GetCart();
        void AddItem(int restaurantId, string restaurantName, int menuItemId, string name, decimal price, int quantity, string specialInstructions);
        void UpdateQuantity(int menuItemId, int quantity);
        void RemoveItem(int menuItemId);
        void ClearCart();
    }
}
