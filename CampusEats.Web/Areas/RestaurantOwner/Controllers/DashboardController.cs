using CampusEats.Core.Enums;
using CampusEats.Core.Interfaces;
using CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusEats.Web.Areas.RestaurantOwner.Controllers
{
    [Area("RestaurantOwner")]
    [Authorize(Roles = "RestaurantOwner")]
    public class DashboardController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IOrderService _orderService;
        private readonly IMenuService _menuService;

        public DashboardController(
            IRestaurantService restaurantService,
            IOrderService orderService,
            IMenuService menuService)
        {
            _restaurantService = restaurantService;
            _orderService = orderService;
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var restaurant = await _restaurantService.GetRestaurantByOwnerIdAsync(userId);

                if (restaurant == null)
                {
                    TempData["Info"] = "Please create your restaurant profile first.";
                    return RedirectToAction("Create", "Settings");
                }

                var today = DateTime.UtcNow.Date;
                var orders = await _orderService.GetRestaurantOrdersAsync(
                    restaurant.RestaurantId,
                    today,
                    today.AddDays(1).AddTicks(-1));

                var menuItems = await _menuService.GetAvailableMenuItemsAsync(restaurant.RestaurantId);

                var viewModel = new DashboardViewModel
                {
                    Restaurant = restaurant,
                    TodayOrders = orders.ToList(),
                    TotalRevenue = orders.Where(o => o.Status != OrderStatus.Cancelled)
                                       .Sum(o => o.TotalAmount),
                    MenuItemCount = menuItems.Count(),
                    PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading the dashboard. " + ex.Message;
                return RedirectToAction("Create", "Settings");
            }
        }
    }
}
