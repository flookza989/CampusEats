using CampusEats.Core.Interfaces;
using CampusEats.Web.Areas.Customer.Interfaces;
using CampusEats.Web.Areas.Customer.Models.Carts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Student,Staff")]
    public class CartController : Controller
    {
        private readonly IShoppingCartService _cartService;
        private readonly IRestaurantService _restaurantService;

        public CartController(
            IShoppingCartService cartService,
            IRestaurantService restaurantService)
        {
            _cartService = cartService;
            _restaurantService = restaurantService;
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddToCartRequest request)
        {
            try
            {
                // ตรวจสอบว่าร้านเปิดอยู่หรือไม่
                var isOpen = await _restaurantService.IsRestaurantOpenAsync(request.RestaurantId);
                if (!isOpen)
                {
                    return Json(new { success = false, message = "The restaurant is closed" });
                }

                _cartService.AddItem(
                    request.RestaurantId,
                    request.RestaurantName,
                    request.MenuItemId,
                    request.Name,
                    request.Price,
                    request.Quantity,
                    request.SpecialInstructions
                );

                var cart = _cartService.GetCart();
                return Json(new
                {
                    success = true,
                    itemCount = cart.Items.Sum(i => i.Quantity)
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while adding the item to the cart"
                });
            }
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            return View(new CartViewModel { Cart = cart });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int menuItemId, int quantity)
        {
            try
            {
                _cartService.UpdateQuantity(menuItemId, quantity);
                var cart = _cartService.GetCart();

                return Json(new
                {
                    success = true,
                    total = cart.Total,
                    itemCount = cart.Items.Sum(i => i.Quantity)
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the item quantity"
                });
            }
        }

        [HttpPost]
        public IActionResult RemoveItem(int menuItemId)
        {
            try
            {
                _cartService.RemoveItem(menuItemId);
                var cart = _cartService.GetCart();

                return Json(new
                {
                    success = true,
                    total = cart.Total,
                    itemCount = cart.Items.Sum(i => i.Quantity)
                });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while removing the item"
                });
            }
        }

        [HttpPost]
        public IActionResult Clear()
        {
            try
            {
                _cartService.ClearCart();
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while clearing the cart"
                });
            }
        }
    }
}
