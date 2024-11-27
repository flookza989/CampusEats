using CampusEats.Web.Interface;
using CampusEats.Web.Models.Order;
using System.Text.Json;

namespace CampusEats.Web.Services
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKey = "Cart";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CartViewModel GetCart()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = session.GetString(CartSessionKey);

            if (string.IsNullOrEmpty(cartJson))
                return new CartViewModel();

            return JsonSerializer.Deserialize<CartViewModel>(cartJson);
        }

        public void AddItem(int restaurantId, string restaurantName, int menuItemId, string name, decimal price, int quantity = 1, string instructions = "")
        {
            var cart = GetCart();

            // Check if adding item from different restaurant
            if (cart.Items.Any() && cart.RestaurantId != restaurantId)
            {
                throw new InvalidOperationException("Cannot add items from different restaurants");
            }

            // Set restaurant if cart is empty
            if (!cart.Items.Any())
            {
                cart.RestaurantId = restaurantId;
                cart.RestaurantName = restaurantName;
            }

            var existingItem = cart.Items.FirstOrDefault(i => i.MenuItemId == menuItemId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                existingItem.SpecialInstructions = instructions;
            }
            else
            {
                cart.Items.Add(new CartItemViewModel
                {
                    MenuItemId = menuItemId,
                    Name = name,
                    Price = price,
                    Quantity = quantity,
                    SpecialInstructions = instructions
                });
            }

            SaveCart(cart);
        }

        public void UpdateItemQuantity(int menuItemId, int quantity)
        {
            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(i => i.MenuItemId == menuItemId);

            if (item != null)
            {
                if (quantity > 0)
                    item.Quantity = quantity;
                else
                    cart.Items.Remove(item);

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
                if (!cart.Items.Any())
                {
                    cart.RestaurantId = 0;
                    cart.RestaurantName = null;
                }
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            _httpContextAccessor.HttpContext.Session.Remove(CartSessionKey);
        }

        public bool HasItems()
        {
            return GetCart().Items.Any();
        }

        private void SaveCart(CartViewModel cart)
        {
            var session = _httpContextAccessor.HttpContext.Session;
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(CartSessionKey, cartJson);
        }
    }
}
