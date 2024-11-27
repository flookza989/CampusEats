using CampusEats.Core.Enums;
using CampusEats.Core.Interfaces;
using CampusEats.Web.Areas.RestaurantOwner.Models.RestaurantOwner;
using CampusEats.Web.Hubs;
using CampusEats.Web.Models.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CampusEats.Web.Areas.RestaurantOwner.Controllers
{
    [Area("RestaurantOwner")]
    [Authorize(Roles = "RestaurantOwner")]
    public class OrderManagementController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IRestaurantService _restaurantService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public OrderManagementController(
            IOrderService orderService,
            IRestaurantService restaurantService,
            INotificationService notificationService,
            IHubContext<NotificationHub> hubContext)
        {
            _orderService = orderService;
            _restaurantService = restaurantService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var restaurant = await _restaurantService.GetRestaurantByOwnerIdAsync(userId);

            var today = DateTime.UtcNow.Date;
            var orders = await _orderService.GetRestaurantOrdersAsync(
                restaurant.RestaurantId,
                today,
                today.AddDays(1).AddTicks(-1));

            var viewModel = new OrderManagementViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                RestaurantName = restaurant.Name,
                Orders = orders.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.User.FullName,
                    OrderTime = o.OrderTime,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    SpecialInstructions = o.SpecialInstructions,
                    Items = o.OrderItems.Select(oi => new Models.RestaurantOwner.OrderItemViewModel
                    {
                        Name = oi.MenuItem.Name,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        SpecialInstructions = oi.SpecialInstructions
                    }).ToList()
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> OrderHistory(DateTime? startDate, DateTime? endDate)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var restaurant = await _restaurantService.GetRestaurantByOwnerIdAsync(userId);

            var orders = await _orderService.GetRestaurantOrdersAsync(
                restaurant.RestaurantId,
                startDate,
                endDate);

            var viewModel = new Models.RestaurantOwner.OrderHistoryViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                RestaurantName = restaurant.Name,
                StartDate = startDate,
                EndDate = endDate,
                Orders = orders.Select(o => new OrderViewModel
                {
                    OrderId = o.OrderId,
                    CustomerName = o.User.FullName,
                    OrderTime = o.OrderTime,
                    Status = o.Status.ToString(),
                    TotalAmount = o.TotalAmount,
                    SpecialInstructions = o.SpecialInstructions,
                    Items = o.OrderItems.Select(oi => new Models.RestaurantOwner.OrderItemViewModel
                    {
                        Name = oi.MenuItem.Name,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity,
                        SpecialInstructions = oi.SpecialInstructions
                    }).ToList()
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
