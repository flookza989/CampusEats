using CampusEats.Core.Interfaces;
using CampusEats.Web.Models.Restaurant;
using Microsoft.AspNetCore.Mvc;

namespace CampusEats.Web.Controllers
{
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

        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var restaurants = await _restaurantService.GetActiveRestaurantsAsync();

            var viewModel = new RestaurantListViewModel
            {
                SearchTerm = searchTerm,
                Restaurants = restaurants.Select(r => new RestaurantSummaryViewModel
                {
                    RestaurantId = r.RestaurantId,
                    Name = r.Name,
                    Description = r.Description,
                    Location = r.Location,
                    IsOpen = r.OpeningTime <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= r.ClosingTime,
                    OpeningTime = r.OpeningTime.ToString(@"hh\:mm"),
                    ClosingTime = r.ClosingTime.ToString(@"hh\:mm"),
                    MenuItemCount = r.MenuItems?.Count ?? 0,
                    FeaturedItems = r.MenuItems?
                        .Where(m => m.IsAvailable)
                        .Take(3)
                        .Select(m => new MenuItemViewModel
                        {
                            ItemId = m.ItemId,
                            Name = m.Name,
                            Description = m.Description,
                            Price = m.Price,
                            ImageUrl = m.ImageUrl,
                            IsAvailable = m.IsAvailable
                        })
                        .ToList() ?? new List<MenuItemViewModel>()
                }).ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var restaurant = await _restaurantService.GetRestaurantDetailsAsync(id);
            var menuItems = await _restaurantService.GetRestaurantMenuAsync(id);

            var viewModel = new RestaurantDetailViewModel
            {
                RestaurantId = restaurant.RestaurantId,
                Name = restaurant.Name,
                Description = restaurant.Description,
                Location = restaurant.Location,
                Phone = restaurant.Phone,
                IsOpen = restaurant.OpeningTime <= DateTime.Now.TimeOfDay && DateTime.Now.TimeOfDay <= restaurant.ClosingTime,
                OpeningTime = restaurant.OpeningTime.ToString(@"hh\:mm"),
                ClosingTime = restaurant.ClosingTime.ToString(@"hh\:mm"),
                MenuItems = menuItems.Select(m => new MenuItemViewModel
                {
                    ItemId = m.ItemId,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    ImageUrl = m.ImageUrl,
                    IsAvailable = m.IsAvailable
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
