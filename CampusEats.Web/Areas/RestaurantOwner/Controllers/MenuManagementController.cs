using CampusEats.Core.Entities;
using CampusEats.Core.Interfaces;
using CampusEats.Web.Areas.RestaurantOwner.Models.Menu;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampusEats.Web.Areas.RestaurantOwner.Controllers
{
    [Area("RestaurantOwner")]
    [Authorize(Roles = "RestaurantOwner")]
    public class MenuManagementController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IRestaurantService _restaurantService;

        [HttpPost]
        public async Task<IActionResult> AddMenuItem(CreateMenuItemViewModel model)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var restaurant = await _restaurantService.GetRestaurantByOwnerIdAsync(userId);

            if (restaurant.RestaurantId != model.RestaurantId)
            {
                return Forbid();
            }

            // Process menu item creation
            var menuItem = new MenuItem
            {
                RestaurantId = model.RestaurantId,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                IsAvailable = model.IsAvailable
            };

            await _menuService.AddMenuItemAsync(menuItem);
            return RedirectToAction("Index");
        }
    }
}
