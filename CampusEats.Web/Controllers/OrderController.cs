using CampusEats.Core.Interfaces;
using CampusEats.Web.Interface;
using CampusEats.Web.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusEats.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IRestaurantService _restaurantService;
        private readonly IUserService _userService;
        private readonly ICartService _cartService;

        public OrderController(
            IOrderService orderService,
            IRestaurantService restaurantService,
            IUserService userService,
            ICartService cartService)
        {
            _orderService = orderService;
            _restaurantService = restaurantService;
            _userService = userService;
            _cartService = cartService;
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var cart = _cartService.GetCart();
            if (!cart.Items.Any())
            {
                return RedirectToAction("Index", "Restaurant");
            }

            var restaurant = await _restaurantService.GetRestaurantDetailsAsync(cart.RestaurantId);
            var user = await _userService.GetUserProfileAsync(GetCurrentUserId());

            var viewModel = new CheckoutViewModel
            {
                RestaurantId = cart.RestaurantId,
                RestaurantName = cart.RestaurantName,
                Items = cart.Items.Select(i => new CheckoutItemViewModel
                {
                    MenuItemId = i.MenuItemId,
                    Name = i.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    SpecialInstructions = i.SpecialInstructions
                }).ToList(),
                UserPhone = user.Phone
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Checkout", model);
            }

            try
            {
                var cart = _cartService.GetCart();
                var orderItems = cart.Items.Select(i =>
                    (i.MenuItemId, i.Quantity, i.SpecialInstructions)).ToList();

                var order = await _orderService.CreateOrderAsync(
                    GetCurrentUserId(),
                    cart.RestaurantId,
                    orderItems);

                _cartService.ClearCart();

                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction(nameof(Details), new { id = order.OrderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Failed to place order. " + ex.Message);
                return View("Checkout", model);
            }
        }

        public async Task<IActionResult> History()
        {
            var orders = await _orderService.GetUserOrderHistoryAsync(GetCurrentUserId());

            var viewModel = new OrderHistoryViewModel
            {
                Orders = orders.Select(o => new OrderSummaryViewModel
                {
                    OrderId = o.OrderId,
                    RestaurantName = o.Restaurant.Name,
                    OrderTime = o.OrderTime,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    ItemCount = o.OrderItems.Count
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetailsAsync(id);
            var userId = GetCurrentUserId();

            if (order.UserId != userId)
            {
                return Forbid();
            }

            var notifications = await _orderService.GetOrderStatusUpdatesAsync(id);

            var viewModel = new OrderDetailViewModel
            {
                OrderId = order.OrderId,
                RestaurantName = order.Restaurant.Name,
                RestaurantPhone = order.Restaurant.Phone,
                OrderTime = order.OrderTime,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                SpecialInstructions = order.SpecialInstructions,
                Items = order.OrderItems.Select(i => new OrderItemViewModel
                {
                    Name = i.MenuItem.Name,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    SpecialInstructions = i.SpecialInstructions
                }).ToList(),
                StatusUpdates = notifications.Select(n => new OrderStatusUpdateViewModel
                {
                    Status = n.Status.ToString(),
                    UpdateTime = n.CreatedAt,
                    Message = n.Message
                }).OrderBy(n => n.UpdateTime).ToList(),
                CanBeCancelled = await _orderService.CanUserCancelOrderAsync(id, userId)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (await _orderService.CanUserCancelOrderAsync(id, userId))
                {
                    await _orderService.CancelOrderAsync(id);
                    TempData["SuccessMessage"] = "Order cancelled successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Cannot cancel this order.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to cancel order: " + ex.Message;
            }

            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
