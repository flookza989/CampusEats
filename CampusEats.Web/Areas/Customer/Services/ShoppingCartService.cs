using CampusEats.Web.Areas.Customer.Interfaces;
using CampusEats.Web.Areas.Customer.Models.Carts;
using System.Text.Json;

namespace CampusEats.Web.Areas.Customer.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _cartKey = "ShoppingCart";

        public ShoppingCartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ShoppingCart GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            string cartJson = session.GetString(_cartKey);
            return cartJson == null ? new ShoppingCart() : JsonSerializer.Deserialize<ShoppingCart>(cartJson);
        }

        public void AddItem(int restaurantId, string restaurantName, int menuItemId, string name, decimal price, int quantity, string specialInstructions)
        {
            var cart = GetCart();

            // ถ้าตะกร้าว่างหรือเป็นร้านเดียวกัน
            if (cart.Items.Count == 0 || cart.RestaurantId == restaurantId)
            {
                cart.RestaurantId = restaurantId;
                cart.RestaurantName = restaurantName;

                var existingItem = cart.Items.FirstOrDefault(i => i.MenuItemId == menuItemId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                    existingItem.SpecialInstructions = specialInstructions;
                }
                else
                {
                    cart.Items.Add(new CartItem
                    {
                        MenuItemId = menuItemId,
                        Name = name,
                        Price = price,
                        Quantity = quantity,
                        SpecialInstructions = specialInstructions
                    });
                }

                SaveCart(cart);
            }
            else
            {
                throw new InvalidOperationException("Cannot order food from multiple restaurants at the same time");
            }
        }

        public void UpdateQuantity(int menuItemId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.MenuItemId == menuItemId);

            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    cart.Items.Remove(item);
                }

                if (cart.Items.Count == 0)
                {
                    cart.RestaurantId = 0;
                    cart.RestaurantName = null;
                }

                SaveCart(cart);
            }
        }

        public void RemoveItem(int menuItemId)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.MenuItemId == menuItemId);

            if (item != null)
            {
                cart.Items.Remove(item);

                if (cart.Items.Count == 0)
                {
                    cart.RestaurantId = 0;
                    cart.RestaurantName = null;
                }

                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            _httpContextAccessor.HttpContext.Session.Remove(_cartKey);
        }

        private void SaveCart(ShoppingCart cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            session.SetString(_cartKey, JsonSerializer.Serialize(cart));
        }
    }
}
