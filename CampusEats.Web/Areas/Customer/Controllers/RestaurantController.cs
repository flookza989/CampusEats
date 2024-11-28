using CampusEats.Core.Interfaces;
using CampusEats.Web.Areas.Customer.Models.Restaurants;
using CampusEats.Web.Models.Restaurant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MenuItemViewModel = CampusEats.Web.Areas.Customer.Models.Restaurants.MenuItemViewModel;
using RestaurantDetailViewModel = CampusEats.Web.Areas.Customer.Models.Restaurants.RestaurantDetailViewModel;
using RestaurantListViewModel = CampusEats.Web.Areas.Customer.Models.Restaurants.RestaurantListViewModel;
using RestaurantSummaryViewModel = CampusEats.Web.Areas.Customer.Models.Restaurants.RestaurantSummaryViewModel;

namespace CampusEats.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Student,Staff")]
    public class RestaurantController : Controller
    {
        private readonly IRestaurantService _restaurantService;
        private readonly IMenuService _menuService;

        public RestaurantController(
            IRestaurantService restaurantService,
            IMenuService menuService)
        {
            _restaurantService = restaurantService;
            _menuService = menuService;
        }

        public async Task<IActionResult> Index(string searchTerm = "", bool showOpenOnly = false)
        {
            try
            {
                var restaurants = await _restaurantService.GetActiveRestaurantsAsync();

                var viewModel = new RestaurantListViewModel
                {
                    SearchTerm = searchTerm,
                    ShowOpenOnly = showOpenOnly,
                    Restaurants = restaurants.Select(r => new RestaurantSummaryViewModel
                    {
                        RestaurantId = r.RestaurantId,
                        Name = r.Name,
                        Description = r.Description,
                        Location = r.Location,
                        IsOpen = IsRestaurantOpen(r.OpeningTime, r.ClosingTime) && r.IsActive,
                        OpeningTimeString = r.OpeningTime.ToString(@"hh\:mm"),
                        ClosingTimeString = r.ClosingTime.ToString(@"hh\:mm")
                    }).ToList()
                };

                // กรองตามคำค้นหา
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    viewModel.Restaurants = viewModel.Restaurants
                        .Where(r => r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                  r.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                // กรองเฉพาะร้านที่เปิด
                if (showOpenOnly)
                {
                    viewModel.Restaurants = viewModel.Restaurants
                        .Where(r => r.IsOpen)
                        .ToList();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "เกิดข้อผิดพลาดในการโหลดรายการร้านอาหาร";
                return RedirectToAction("Index", "Home", new { area = "" });
            }
        }

        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var restaurant = await _restaurantService.GetRestaurantDetailsAsync(id);
                if (restaurant == null)
                    return NotFound();

                var menuItems = await _menuService.GetAvailableMenuItemsAsync(id);

                // จัดกลุ่มเมนูตามประเภท (สมมติว่าใช้คำอธิบายเป็นประเภท)
                var groupedItems = menuItems.GroupBy(m => m.Description ?? "อื่นๆ")
                    .Select(g => new MenuCategoryViewModel
                    {
                        Name = g.Key,
                        Items = g.Select(m => new MenuItemViewModel
                        {
                            ItemId = m.ItemId,
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Price,
                            ImageUrl = m.ImageUrl,
                            IsAvailable = m.IsAvailable
                        }).ToList()
                    }).ToList();

                var viewModel = new RestaurantDetailViewModel
                {
                    RestaurantId = restaurant.RestaurantId,
                    Name = restaurant.Name,
                    Description = restaurant.Description,
                    Location = restaurant.Location,
                    Phone = restaurant.Phone,
                    IsOpen = IsRestaurantOpen(restaurant.OpeningTime, restaurant.ClosingTime) && restaurant.IsActive,
                    OpeningTime = restaurant.OpeningTime,
                    ClosingTime = restaurant.ClosingTime,
                    Categories = groupedItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "เกิดข้อผิดพลาดในการโหลดข้อมูลร้านอาหาร";
                return RedirectToAction(nameof(Index));
            }
        }

        private bool IsRestaurantOpen(TimeSpan openingTime, TimeSpan closingTime)
        {
            var currentTime = DateTime.Now.TimeOfDay;
            return currentTime >= openingTime && currentTime <= closingTime;
        }
    }
}
